<?xml version="1.0" encoding="utf-8"?>

<!--
  This script originally came from the "tinesware-tools" project at
  http://code.google.com/p/tinesware-tools/
  
  It is licensed under the MIT license
  http://www.opensource.org/licenses/mit-license.php
  
  In short, you can copy, edit, publish or sell it at your leisure.
  You must not sue its contributors for damages and remove the copyright.
  Read the full license for details.
-->
<xsl:stylesheet
  version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  xmlns:user="urn:my-scripts"
>

  <xsl:output method="html" indent="no" />

  <msxsl:script language="C#" implements-prefix="user">
    <![CDATA[
      public string filename(string path) {
        return (new System.IO.FileInfo(path)).Name;
      }
    ]]>
  </msxsl:script>

  <msxsl:script language="C#" implements-prefix="user">
    <![CDATA[
      public string identity(string path){
        if (System.IO.File.Exists(path)) {
          try {
            return System.Reflection.Assembly.LoadFile(path).FullName;
          }  catch (Exception) {}
        }

        return "?";
      }
    ]]>
  </msxsl:script>

  <xsl:template match="PartCoverReport">
    <coverage profilerVersion="{@version}" driverVersion="{@version}" startTime="{@date}" measureTime="{@date}">
      <xsl:for-each select="//Assembly">
        <xsl:variable name="assemblyRef" select="@id" />
        <module moduleId="{@id}" assembly="{@name}">
          <xsl:attribute name="name">
            <xsl:value-of select="user:filename(@module)"/>
          </xsl:attribute>
          <xsl:attribute name="assemblyIdentity">
            <xsl:value-of select="user:identity(@module)"/>
          </xsl:attribute>
          <xsl:for-each select="//Method">
            <xsl:choose>
              <xsl:when test="../@asmref = $assemblyRef">
                <method name="{@name}" class ="{../@name}" sig="{@sig}" excluded="false" instrumented="true">

                  <xsl:for-each select="./pt">
                    <xsl:choose>
                      <xsl:when test="@sl != ''">
                        <xsl:variable name="fileRef" select="@fid" />
                        <xsl:variable name="file" select="//File[@id = $fileRef]/@url" />
                        <seqpnt visitcount="{@visit}" line="{@sl}" column="{@sc}" endline="{@el}" endcolumn="{@ec}" excluded="false">
                          <xsl:attribute name="document">
                            <xsl:value-of select="$file"/>
                          </xsl:attribute>
                        </seqpnt>
                      </xsl:when>
                    </xsl:choose>
                  </xsl:for-each>
                </method>
              </xsl:when>
            </xsl:choose>
          </xsl:for-each>
        </module>
      </xsl:for-each>
    </coverage>
  </xsl:template>

</xsl:stylesheet>
