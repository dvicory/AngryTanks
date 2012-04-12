#pragma region CPL License
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
#pragma endregion

#pragma once

#include "SpriteFontContent.h"

namespace Nuclex { namespace Fonts { namespace Content {

  /// <summary>Serializes SpriteFontContent resources</summary>
  /// <remarks>
  ///   This class is responsible for serializing the FontContent resources into
  ///   the XNA framework format.
  /// </remarks>
  [
    Microsoft::Xna::Framework::Content::Pipeline::Serialization::Compiler::ContentTypeWriter
  ]
  public ref class SpriteFontWriter :
    Microsoft::Xna::Framework::Content::Pipeline::Serialization::Compiler::ContentTypeWriter<
      Nuclex::Fonts::Content::SpriteFontContent ^
    > {

    /// <summary>Writes the FontContent resource into the ContentWriter</summary>
    /// <param name="output">ContentWriter to serialize the resource into</param>
    /// <param name="fontContent">FontContent resource to be serialized</param>
    protected: virtual void Write(
      Microsoft::Xna::Framework::Content::Pipeline::Serialization::Compiler::ContentWriter ^output,
      Nuclex::Fonts::Content::SpriteFontContent ^fontContent
    ) override;

    /// <param name="targetPlatform">Platform for which to obtain the RuntimeReader</param>
    /// <returns>The type name of the class used to read the resource at runtime</returns>
    public: virtual System::String ^GetRuntimeReader(
#if defined(XNA_4)
      Microsoft::Xna::Framework::Content::Pipeline::TargetPlatform targetPlatform
#else
      Microsoft::Xna::Framework::TargetPlatform targetPlatform
#endif
    ) override;

    /// <summary>Obtains the Type of the RuntimeReader used to load the resource</summary>
    /// <param name="targetPlatform">Platform for which to obtain the RuntimeReader</param>
    /// <returns>The type name of the class used to read the resource at runtime</returns>
    public: virtual System::String ^GetRuntimeType(
#if defined(XNA_4)
      Microsoft::Xna::Framework::Content::Pipeline::TargetPlatform targetPlatform
#else
      Microsoft::Xna::Framework::TargetPlatform targetPlatform
#endif
    ) override;

  };

}}} // namespace Nuclex::Fonts::Content
