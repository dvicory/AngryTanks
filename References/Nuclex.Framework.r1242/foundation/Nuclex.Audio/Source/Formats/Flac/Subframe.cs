#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2009 Nuclex Development Labs

This library is free software; you can redistribute it and/or
modify it under the terms of the IBM Common Public License as
published by the IBM Corporation; either version 1.0 of the
License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
IBM Common Public License for more details.

You should have received a copy of the IBM Common Public
License along with this library
*/
#endregion

using System;
using System.Collections.Generic;

namespace Nuclex.Audio.Formats.Flac {

#if ENABLE_PINVOKE_FLAC_DECODER

  /// <summary>CONSTANT subframe.</summary>
  public struct ConstantSubframe {

    /// <summary>The constant signal value.</summary>
    public Int32 Value;

  };

  /// <summary>VERBATIM subframe.</summary>
  public struct VerbatimSubframe {

    /// <summary>A pointer to verbatim signal.</summary>
    public IntPtr Data; // int32 *

  }

  /// <summary>FIXED subframe.</summary>
  public struct FixedSubframe {

    /// <summary>Describes the entropy coding method used</summary>
    public EntropyCodingMethod EntropyCodingMethod;

    /// <summary>Polynomial order</summary>
    public int Order;

    /// <summary>
    ///   Warmup samples to prime the predictor, length == <see cref="Order" />
    /// </summary>
    public int[] Warmup;

    /// <summary>
    ///   The residual signal, length == (blocksize minus order) samples.
    /// </summary>
    public int[] Residual;

  }

  /// <summary>LPC subframe.</summary>
  public struct LpcSubframe {

    /// <summary>The residual coding method.</summary>
    public EntropyCodingMethod EntropyCodingMethod;

    /// <summary>The FIR order.</summary>
    public int Order;

    /// <summary>Quantized FIR filter coefficient precision in bits.</summary>
    public int QlpCoefficientPrecision;

    /// <summary>The qlp coeff shift needed.</summary>
    public int QuantizationLevel;

    /// <summary>FIR filter coefficients.</summary>
    public int[] QlpCoefficients;

    /// <summary>Warmup samples to prime the predictor, length == order.</summary>
    public int[] Warmup;

    /// <summary>
    ///   The residual signal, length == (blocksize minus order) samples.
    /// </summary>
    public int[] Residual;

  }

  /// <summary>FLAC subframe structure</summary>
  public struct Subframe {

    /// <summary>Type of the subframe for this channel</summary>
    public SubframeType Type;

    /// <summary>Constant data for the subframe</summary>
    public ConstantSubframe? Constant;
    /// <summary>Fixed data for the subframe</summary>
    public FixedSubframe? Fixed;
    /// <summary>LPC data for the subframe</summary>
    public LpcSubframe? Lpc;
    /// <summary>Verbatim data for the subframe</summary>
    public VerbatimSubframe? Verbatim;

    /// <summary>Unknown content</summary>
    public int WastedBits;

  }

#endif // ENABLE_PINVOKE_FLAC_DECODER


} // namespace Nuclex.Audio.Formats.Flac
