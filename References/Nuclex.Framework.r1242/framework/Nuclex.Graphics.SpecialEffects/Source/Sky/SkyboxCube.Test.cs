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

#if UNITTEST

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;

using Nuclex.Testing.Xna;

namespace Nuclex.Graphics.SpecialEffects.Sky {

  /// <summary>Unit tests for the skybox cube class</summary>
  [TestFixture]
  internal class SkyboxCubeTest {

    /// <summary>
    ///   Verifies that the constructor of the skybox cube class is working
    /// </summary>
    [Test]
    public void TestConstructor() {
      MockedGraphicsDeviceService mockGraphicsDeviceService =
        new MockedGraphicsDeviceService();

      using(IDisposable keeper = mockGraphicsDeviceService.CreateDevice()) {
        SkyboxCube theSkybox = new SkyboxCube(mockGraphicsDeviceService.GraphicsDevice);
        theSkybox.Dispose();
      }
    }

    /// <summary>
    ///   Verifies that the skybox cube can render a skybox
    /// </summary>
    [Test]
    public void TestRenderSkybox() {
      MockedGraphicsDeviceService mockGraphicsDeviceService =
        new MockedGraphicsDeviceService(DeviceType.Reference);

      using(IDisposable keeper = mockGraphicsDeviceService.CreateDevice()) {
        using(
          BasicEffect effect = new BasicEffect(
#if XNA_4
            mockGraphicsDeviceService.GraphicsDevice
#else
            mockGraphicsDeviceService.GraphicsDevice, new EffectPool()
#endif
          )
        ) {
          using(
            SkyboxCube skyboxCube = new SkyboxCube(
              mockGraphicsDeviceService.GraphicsDevice
            )
          ) {
            skyboxCube.AssignVertexBuffer();
#if XNA_4
            EffectTechnique technique = effect.CurrentTechnique;
            for(int pass = 0; pass < technique.Passes.Count; ++pass) {
              technique.Passes[pass].Apply();

              skyboxCube.DrawNorthernFace();
              skyboxCube.DrawEasternFace();
              skyboxCube.DrawSouthernFace();
              skyboxCube.DrawWesternFace();
              skyboxCube.DrawUpperFace();
              skyboxCube.DrawLowerFace();
            }
#else
            effect.Begin();
            try {
              EffectTechnique technique = effect.CurrentTechnique;
              for(int pass = 0; pass < technique.Passes.Count; ++pass) {
                technique.Passes[pass].Begin();
                try {
                  skyboxCube.DrawNorthernFace();
                  skyboxCube.DrawEasternFace();
                  skyboxCube.DrawSouthernFace();
                  skyboxCube.DrawWesternFace();
                  skyboxCube.DrawUpperFace();
                  skyboxCube.DrawLowerFace();
                }
                finally {
                  technique.Passes[pass].End();
                }
              }
            }
            finally {
              effect.End();
            }
#endif
          }
        }
      }
    }

  }

} // namespace Nuclex.Graphics.SpecialEffects.Sky

#endif // UNITTEST
