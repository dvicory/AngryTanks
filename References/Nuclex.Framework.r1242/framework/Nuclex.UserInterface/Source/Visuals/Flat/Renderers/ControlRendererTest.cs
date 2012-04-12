#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2010 Nuclex Development Labs

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
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using NUnit.Framework;
using NMock2;

namespace Nuclex.UserInterface.Visuals.Flat.Renderers {

  /// <summary>Base class for control renderer unit tests</summary>
  public abstract class ControlRendererTest {

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public virtual void Setup() {
      this.mockery = new Mockery();
      this.mockedGraphics = this.mockery.NewMock<IFlatGuiGraphics>();
      
      this.screen = new Screen(800, 600);
    }
    
    /// <summary>Called after each test is run</summary>
    [TearDown]
    public virtual void TearDown() {
      if(this.mockery != null) {
        this.screen = null;
        this.mockedGraphics = null;
      
        this.mockery.Dispose();
        this.mockery = null;
      }
    }
    
    /// <summary>Mockery used to create the mocked graphics interface</summary>
    protected Mockery Mockery {
      get { return this.mockery; }
    }
    
    /// <summary>Mocked graphics interface the GUI can be tested on</summary>
    protected IFlatGuiGraphics MockedGraphics {
      get { return this.mockedGraphics; }
    }

    /// <summary>Screen the controls can be placed on for testing</summary>
    protected Screen Screen {
      get { return this.screen; }
    }
    
    /// <summary>Screen the tested control can be placed on</summary>
    private Screen screen;
    
    /// <summary>Graphics interface used for rendering the GUI</summary>
    private IFlatGuiGraphics mockedGraphics;
    
    /// <summary>Mockery used to create the mocked graphics interface</summary>
    private Mockery mockery;

  }

} // namespace Nuclex.UserInterface.Visuals.Flat.Renderers

#endif // UNITTEST
