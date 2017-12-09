using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FwColorDialog = System.Windows.Forms.ColorDialog;

namespace Cyotek.Windows.Forms
{
  // Cyotek Color Picker controls library
  // Copyright © 2013-2017 Cyotek Ltd.
  // http://cyotek.com/blog/tag/colorpicker

  // Licensed under the MIT License. See license.txt for the full text.

  // If you use this code in your applications, donations or attribution are welcome

  [DefaultEvent("PreviewColorChanged")]
  [DefaultProperty("Color")]
  public partial class ColorPickerDialog : Form
  {
    #region Constants

    private static readonly object _eventLoadPalette = new object();

    private static readonly object _eventPreviewColorChanged = new object();

    private static readonly object _eventSavePalette = new object();

    #endregion

    #region Fields

    private bool _showAlphaChannel;

    private bool _showLoadPalette;

    private bool _showSavePalette;

    private Brush _textureBrush;

    #endregion

    #region Constructors

    public ColorPickerDialog()
    {
      this.InitializeComponent();
      _showAlphaChannel = true;
      base.Font = SystemFonts.DialogFont;
    }

    #endregion

    #region Events

    [Category("Action")]
    public event CancelEventHandler LoadPalette
    {
      add { this.Events.AddHandler(_eventLoadPalette, value); }
      remove { this.Events.RemoveHandler(_eventLoadPalette, value); }
    }

    [Category("Property Changed")]
    public event EventHandler PreviewColorChanged
    {
      add { this.Events.AddHandler(_eventPreviewColorChanged, value); }
      remove { this.Events.RemoveHandler(_eventPreviewColorChanged, value); }
    }

    [Category("Action")]
    public event CancelEventHandler SavePalette
    {
      add { this.Events.AddHandler(_eventSavePalette, value); }
      remove { this.Events.RemoveHandler(_eventSavePalette, value); }
    }

    #endregion

    #region Properties

    public Color Color
    {
      get { return colorEditorManager.Color; }
      set { colorEditorManager.Color = value; }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ShowAlphaChannel
    {
      get { return _showAlphaChannel; }
      set { _showAlphaChannel = value; }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ShowLoadPalette
    {
      get { return _showLoadPalette; }
      set
      {
        if (_showLoadPalette != value)
        {
          _showLoadPalette = value;

          loadPaletteButton.Visible = value;
        }
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ShowSavePalette
    {
      get { return _showLoadPalette; }
      set
      {
        if (_showSavePalette != value)
        {
          _showSavePalette = value;

          savePaletteButton.Visible = value;
        }
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (components != null)
        {
          components.Dispose();
        }

        if (_textureBrush != null)
        {
          _textureBrush.Dispose();
          _textureBrush = null;
        }
      }

      base.Dispose(disposing);
    }

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data. </param>
    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      savePaletteButton.Visible = _showSavePalette;
      loadPaletteButton.Visible = _showLoadPalette;
      colorEditor.ShowAlphaChannel = _showAlphaChannel;

      if (!_showAlphaChannel)
      {
        colorGrid.BeginUpdate();

        for (int i = 0; i < colorGrid.Colors.Count; i++)
        {
          Color color;

          color = colorGrid.Colors[i];

          if (color.A != 255)
          {
            colorGrid.Colors[i] = Color.FromArgb(255, color);
          }
        }

        colorGrid.EndUpdate();
      }
    }

    /// <summary>
    /// Raises the <see cref="LoadPalette" /> event.
    /// </summary>
    /// <param name="e">The <see cref="CancelEventArgs" /> instance containing the event data.</param>
    protected virtual void OnLoadPalette(CancelEventArgs e)
    {
      CancelEventHandler handler;

      handler = (CancelEventHandler)this.Events[_eventLoadPalette];

      handler?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="PreviewColorChanged" /> event.
    /// </summary>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected virtual void OnPreviewColorChanged(EventArgs e)
    {
      EventHandler handler;

      handler = (EventHandler)this.Events[_eventPreviewColorChanged];

      handler?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="SavePalette" /> event.
    /// </summary>
    /// <param name="e">The <see cref="CancelEventArgs" /> instance containing the event data.</param>
    protected virtual void OnSavePalette(CancelEventArgs e)
    {
      CancelEventHandler handler;

      handler = (CancelEventHandler)this.Events[_eventSavePalette];

      handler?.Invoke(this, e);
    }

    private void cancelButton_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }

    private void colorEditorManager_ColorChanged(object sender, EventArgs e)
    {
      previewPanel.Invalidate();

      this.OnPreviewColorChanged(e);
    }

    private void colorGrid_EditingColor(object sender, EditColorCancelEventArgs e)
    {
      e.Cancel = true;

      using (FwColorDialog dialog = new FwColorDialog
                                  {
                                    FullOpen = true,
                                    Color = e.Color
                                  })
      {
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
          colorGrid.Colors[e.ColorIndex] = dialog.Color;
        }
      }
    }

    private void loadPaletteButton_Click(object sender, EventArgs e)
    {
      CancelEventArgs args;

      args = new CancelEventArgs();

      this.OnLoadPalette(args);

      if (!args.Cancel)
      { }
    }

    private void okButton_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void previewPanel_Paint(object sender, PaintEventArgs e)
    {
      Rectangle region;

      region = previewPanel.ClientRectangle;

      if (this.Color.A != 255)
      {
        if (_textureBrush == null)
        {
          using (Bitmap background = new Bitmap(this.GetType().
                                                     Assembly.GetManifestResourceStream(string.Concat(this.GetType().
                                                                                                           Namespace, ".Resources.cellbackground.png"))))
          {
            _textureBrush = new TextureBrush(background, WrapMode.Tile);
          }
        }

        e.Graphics.FillRectangle(_textureBrush, region);
      }

      using (Brush brush = new SolidBrush(this.Color))
      {
        e.Graphics.FillRectangle(brush, region);
      }

      e.Graphics.DrawRectangle(SystemPens.ControlText, region.Left, region.Top, region.Width - 1, region.Height - 1);
    }

    private void savePaletteButton_Click(object sender, EventArgs e)
    {
      CancelEventArgs args;

      args = new CancelEventArgs();

      this.OnSavePalette(args);

      if (!args.Cancel)
      { }
    }

    #endregion
  }
}
