#!/usr/bin/perl

# $Id: ARCue.pl 40 2007-05-21 10:44:04Z Chris $

###############################################################################
#                                                                             #
#                                  ARCue.pl                                   #
#                                                                             #
#                      Christopher Key <cjk32@cam.ac.uk>                      #
#                                                                             #
#                                                                             #
# Many thanks to Mr. Spoon for his kind permission to use the AccurateRip     #
# technology and database.  Access to AccurateRip is regulated, see           #
# http://www.accuraterip.com/3rdparty-access.htm for details.                 #
#                                                                             #
#                                                                             #
# This script allows the usage of AccurateRip's extensive track checksum      #
# database to verify the accuracy of whole CD single file rips. To use,       #
# simply run:                                                                 #
#                                                                             #
# ARCue.pl  cd1.cue  cd2.cue  etc...                                          #
#                                                                             #
###############################################################################

use strict;

use LWP;
use Carp;

my $lwpUserAgent = LWP::UserAgent->new;

foreach my $cueFile (@ARGV) {

	# Parse each cue file, and extract track offsets
	# and the relevant wav file.
	print $cueFile . ":\n\n";

	my $FH;
	open($FH, "<", $cueFile) or croak("Failed to open $cueFile: $!");

	my ($wavFile, $trackCount, $curTrack);
	my @trackOffsets;
	while (<$FH>) {
		chomp;
		if (m/^FILE\s+\"(.+)\"\s+(.+)/) {
			if ($2 ne "WAVE") { croak("Failed to open $cueFile: $!") }
			$wavFile = $1;
		}elsif (m/\s+TRACK\s+(\d+)\s+(.+)/) {
			$curTrack = $1 + 0;
			# What to do with non AUDIO tracks ($2)?
		}elsif (m/\s+INDEX\s+01\s+(\d+):(\d+):(\d+)/) {
			$trackOffsets[$curTrack-1] = ($1*60+$2)*75+$3;
		}
	}
	$trackCount = $curTrack;

	close ($FH);


	# Open the reference wav file, and make sure it looks
	# like one.
	open ($FH, "<", $wavFile) or croak("Failed to open $wavFile: $!");
	binmode $FH;

	my ($chunkID, $chunkSize, $chunkFormat);

	read($FH, $chunkID, 4);
	read($FH, $chunkSize, 4);
	read($FH, $chunkFormat, 4);

	if ($chunkID ne "RIFF" || $chunkFormat ne "WAVE") { croak("$wavFile doesn't appear to be a valid wav file.") }


	# Find the offset to the data in the wav file, and also the start of the leadout
	my ($subChunkID, $subChunkSize, $dataOffset);

	while (!eof($FH)){
		read($FH, $subChunkID, 4);
		read($FH, $subChunkSize, 4);
		$subChunkSize = unpack("V", $subChunkSize);
		if ($subChunkID eq "data") {
			$trackOffsets[$trackCount] = $subChunkSize / 2352; #leadout location
			$dataOffset = tell($FH);
			last;
		}
	}


	# Calculate the length of each track
	my @trackLengths = ();

	for (my $trackNo = 0; $trackNo < $trackCount; $trackNo++) {
		$trackLengths[$trackNo] = $trackOffsets[$trackNo+1] - $trackOffsets[$trackNo];
	}

	# Calculate the three disc ids used by AR
	my ($discId1, $discId2, $cddbDiscId) = (0, 0, 0);

	{
		use integer;

		for (my $trackNo = 0; $trackNo <= $trackCount; $trackNo++) {
			my $trackOffset = $trackOffsets[$trackNo];

			$discId1 += $trackOffset;
			$discId2 += ($trackOffset ? $trackOffset : 1) * ($trackNo + 1);
			if ($trackNo < $trackCount) {
				$cddbDiscId = $cddbDiscId + sumDigits(int($trackOffset/75) + 2);
			}
		}

		$cddbDiscId =
		  (($cddbDiscId % 255) << 24) +
		  ((int($trackOffsets[$trackCount]/75) - int($trackOffsets[0]/75)) << 8) +
		  $trackCount;

		$discId1 &= 0xFFFFFFFF;
		$discId2 &= 0xFFFFFFFF;
		$cddbDiscId &= 0xFFFFFFFF;
	}

  print "Checking AccurateRip database\n\n";

	# See if we can find the disc in the database
	my $arUrl = sprintf("http://www.accuraterip.com/accuraterip/%.1x/%.1x/%.1x/dBAR-%.3d-%.8x-%.8x-%.8x.bin", 
		$discId1 & 0xF, $discId1>>4 & 0xF, $discId1>>8 & 0xF, $trackCount, $discId1, $discId2, $cddbDiscId);

	my $arDiscNotInDb = 0;
	my $arNetworkFailed = 0;

	my $response = $lwpUserAgent->get($arUrl);

	if (!$response->is_success) {
		if ($response->status_line =~ m/^404/) {
			$arDiscNotInDb = 1;
		}else{
			$arNetworkFailed = $response->status_line;
		}
	}

	# Extract CRCs from response data
	my $arCrcCount = 0;
	my @arTrackConfidences = ();
	my @arTrackCRCs = ();

	if (!($arDiscNotInDb || $arNetworkFailed)) {
		my $arCrcData = $response->content;
		my $ptr = 0;

		while ($ptr < length($arCrcData)) {
			my ($chunkTrackCount, $chunkDiscId1, $chunkDiscId2, $chunkCddbDiscId);

			# Force perl to interpret these values as signed integers
			{
				use integer;

				$chunkTrackCount = unpack("c",substr($arCrcData,$ptr,1));
				$chunkDiscId1 = unpack("V",substr($arCrcData,$ptr+1,4)) + 0;
				$chunkDiscId2 = unpack("V",substr($arCrcData,$ptr+5,4)) + 0;
	 			$chunkCddbDiscId = unpack("V",substr($arCrcData,$ptr+9,4)) + 0;
			}

			$ptr +=13;

			if ( $chunkTrackCount != $trackCount
				|| $chunkDiscId1 != $discId1
				|| $chunkDiscId2 != $discId2
				|| $chunkCddbDiscId != $cddbDiscId ) {

				croak("Track count or Disc IDs don't match.");
			}

			# How if it flagged that a track is not in the database?
			for (my $track = 0; $track < $trackCount; $track++) {
				my ($trackConfidence, $trackCrc);

				# Force perl to interpret these values as signed integers
				{
					use integer;

					$trackConfidence = unpack("c",substr($arCrcData,$ptr,1));
					$trackCrc = unpack("V",substr($arCrcData,$ptr+1,4)) + 0;
					$ptr += 9;
				}

				if ($arCrcCount == 0){
					$arTrackConfidences[$track] = [];
					$arTrackCRCs[$track] = [];
				}

				$arTrackConfidences[$track]->[$arCrcCount] = $trackConfidence;
				$arTrackCRCs[$track]->[$arCrcCount] = $trackCrc;
			}
			$arCrcCount++;
		}
	}

	printf "Track\tRipping Status\t\t[Disc ID: %08x-%08x]\n\n", $discId1, $cddbDiscId;

	# Calculate a CRC for each track
	my @trackCRCs = ();
	my ($accuratelyRipped, $notAccuratelyRipped, $notInDatabase) = (0, 0, 0);
	for (my $trackNo = 0; $trackNo < $trackCount; $trackNo++) {
		seek($FH, $dataOffset + $trackOffsets[$trackNo] * 2352, 0);

		my ($frame, $CRC);
		$CRC = 0;
		for (my $frameNo = 0; $frameNo < $trackLengths[$trackNo]; $frameNo++) {
			if (read($FH, $frame, 2352) != 2352) { croak ("read failed.") };

			{
				use integer;
				$CRC += processFrame($frame, $frameNo, $trackLengths[$trackNo], $trackNo == 0, $trackNo == $trackCount - 1);
			}
		}

		{
			use integer;
			$trackCRCs[$trackNo] = $CRC & 0xFFFFFFFF;
		}


		if ($arDiscNotInDb) {
			printf " %d\tTrack not present in database.    [%08x]\n", 
				$trackNo + 1, $trackCRCs[$trackNo];

			 $notInDatabase++;
		}	elsif ($arNetworkFailed) {
			printf " %d\t    [%08x]\n", 
				$trackNo + 1, $trackCRCs[$trackNo];

		} else {

			my $foundCrc = 0;
			my $foundCrcMatch = 0;

			for (my $arCrcNo=0; $arCrcNo < $arCrcCount; $arCrcNo++) {
				if ($arTrackConfidences[$trackNo]->[$arCrcNo] != 0){
					$foundCrc = 1;

					if ($arTrackCRCs[$trackNo]->[$arCrcNo] == $trackCRCs[$trackNo]) {
						printf " %d\tAccurately Ripped    (confidence %d)     [%08x]\n", 
							$trackNo + 1, $arTrackConfidences[$trackNo]->[$arCrcNo], $arTrackCRCs[$trackNo]->[$arCrcNo];

						$accuratelyRipped++;

						$foundCrcMatch = 1;
						last;
					}
				}
			}
			if (!$foundCrc) {
					printf " %d\tTrack not present in database.    [%08x]\n", 
					$trackNo + 1, $trackCRCs[$trackNo];

				$notInDatabase++;
			}elsif (!$foundCrcMatch) {
				printf " %d\t** Rip not accurate **   (confidence %d)     [%08x] [%08x]\n", 
					$trackNo + 1, $arTrackConfidences[$trackNo]->[0], $arTrackCRCs[$trackNo]->[0], $trackCRCs[$trackNo];

				$notAccuratelyRipped++;
			}
		}
	}

	print "\n_______________________\n\n";

	my $errLevel = 0;

	if ($arDiscNotInDb) {
			print "Disc not present in AccurateRip database.\n";
			$errLevel = 2;
	} elsif ($arNetworkFailed) {
			print "Failed to get $arUrl : " . $arNetworkFailed . "\n";
			$errLevel = 3;
	} elsif ($accuratelyRipped == $trackCount) {
		print "All Tracks Accurately Ripped.\n";
	} else {
		if ($notAccuratelyRipped >= 3) {
			 print "Your CD disc is possibly a different pressing to the one(s) stored in AccurateRip.\n"
		}

		printf "Track(s) Accurately Ripped: %d\n", $accuratelyRipped;
		printf "**** Track(s) Not Ripped Accurately: %d ****\n", $notAccuratelyRipped;
		printf "Track(s) Not in Database: %d\n", $notInDatabase;

		$errLevel = 1;
	}

	print "\n\n\n";

	exit $errLevel;
}

sub processFrame {
	use integer;
	my ($frame, $frameNo, $trackLength, $firstTrack, $lastTrack) = @_;

	if ($firstTrack && $frameNo<4) {
		return 0;
	} elsif ($lastTrack && $trackLength - $frameNo < 6) {
		return 0;
	} elsif ($firstTrack && $frameNo == 4) {
		my $sample = unpack("V", substr($frame,2348,4));
		return ($sample * 588 * 5);
	} else {

		my $CRC = 0;
		my $frameOffset = $frameNo * 588;

		foreach (unpack("V588", $frame)) {
			$CRC += $_ * (++$frameOffset);
		}

		return $CRC;
	}
}

sub sumDigits {
	my $n = shift;
	my $r = 0;
	while ($n > 0) {
		$r = $r + ($n % 10);
		$n = int($n / 10);
	}
	return $r;
}
