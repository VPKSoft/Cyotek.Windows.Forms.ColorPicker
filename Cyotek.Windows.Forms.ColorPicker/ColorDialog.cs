using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Cyotek.Windows.Forms
{
  [DefaultEvent(nameof(ColorDialog.ColorChanged))]
  [DefaultProperty(nameof(ColorDialog.Color))]
  public class ColorDialog : Component
  {
    #region Constants

    private static readonly object _eventApplicationLoadPalette = new object();

    private static readonly object _eventApplicationSavePalette = new object();

    private static readonly object _eventColorChanged = new object();

    private static readonly object _eventHelpRequest = new object();

    private static readonly object _eventPreviewColorChanged = new object();

    private static readonly object _lock = new object();

    #endregion

    #region Fields

    private Color _color;

    private bool _showAlphaChannel;

    private bool _showHelp;

    private object _tag;

    #endregion

    #region Constructors

    public ColorDialog()
    {
      _showAlphaChannel = true;
      _color = Color.Black;
    }

    #endregion

    #region Events

    public static event EventHandler ApplicationLoadPalette
    {
      add { AddEventHandler(_eventApplicationLoadPalette, value); }
      remove { RemoveEventHandler(_eventApplicationLoadPalette, value); }
    }

    public static event EventHandler ApplicationSavePalette
    {
      add { AddEventHandler(_eventApplicationSavePalette, value); }
      remove { RemoveEventHandler(_eventApplicationSavePalette, value); }
    }

    /// <summary>
    /// Occurs when the Color property value changes
    /// </summary>
    [Category("Property Changed")]
    public event EventHandler ColorChanged
    {
      add { this.Events.AddHandler(_eventColorChanged, value); }
      remove { this.Events.RemoveHandler(_eventColorChanged, value); }
    }

    [Category("Action")]
    public event EventHandler HelpRequest
    {
      add { this.Events.AddHandler(_eventHelpRequest, value); }
      remove { this.Events.RemoveHandler(_eventHelpRequest, value); }
    }

    [Category("Action")]
    public event EventHandler PreviewColorChanged
    {
      add { this.Events.AddHandler(_eventPreviewColorChanged, value); }
      remove { this.Events.RemoveHandler(_eventPreviewColorChanged, value); }
    }

    #endregion

    #region Static Methods

    internal static void RaiseApplicationLoadPalette(object sender)
    {
      RaiseEvent(sender, _eventApplicationLoadPalette);
    }

    internal static void RaiseApplicationSavePalette(object sender)
    {
      RaiseEvent(sender, _eventApplicationSavePalette);
    }

    private static void AddEventHandler(object key, Delegate value)
    {
      lock (_lock)
      {
        if (_eventHandlers == null)
        {
          _eventHandlers = new EventHandlerList();
        }

        _eventHandlers.AddHandler(key, value);
      }
    }

    private static void RaiseEvent(object sender, object key)
    {
      lock (_lock)
      {
        if (_eventHandlers != null)
        {
          EventHandler handler;

          handler = (EventHandler)_eventHandlers[key];

          handler?.Invoke(sender, EventArgs.Empty);
        }
      }
    }

    private static void RemoveEventHandler(object key, Delegate value)
    {
      lock (_lock)
      {
        _eventHandlers?.RemoveHandler(key, value);
      }
    }

    #endregion

    #region Properties

    [Category("Data")]
    [DefaultValue(typeof(Color), "Black")]
    public Color Color
    {
      get { return _color; }
      set
      {
        if (_color != value)
        {
          _color = value;

          this.OnColorChanged(EventArgs.Empty);
        }
      }
    }

    [DefaultValue(true)]
    [Category("Behavior")]
    public bool ShowAlphaChannel
    {
      get { return _showAlphaChannel; }
      set { _showAlphaChannel = value; }
    }

    [Category("Behavior")]
    [DefaultValue(false)]
    public bool ShowHelp
    {
      get { return _showHelp; }
      set { _showHelp = value; }
    }

    [Category("Data")]
    [DefaultValue(null)]
    [TypeConverter(typeof(StringConverter))]
    public object Tag
    {
      get { return _tag; }
      set { _tag = value; }
    }

    #endregion

    #region Methods

    public DialogResult ShowDialog()
    {
      return this.ShowDialog(null);
    }

    public DialogResult ShowDialog(IWin32Window owner)
    {
      DialogResult result;

      using (ColorPickerDialog dialog = new ColorPickerDialog
      {
        ShowAlphaChannel = _showAlphaChannel,
        Color = _color
      })
      {
        dialog.PreviewColorChanged += this.DialogPreviewColorChangedHandler;

        try
        {
          result = dialog.ShowDialog(owner);

          if(result == DialogResult.OK)
          {
            this.Color = dialog.Color;
          }
        }
        finally
        {
          dialog.PreviewColorChanged -= this.DialogPreviewColorChangedHandler;
        }
      }

      return result;
    }

    private void DialogPreviewColorChangedHandler(object sender, EventArgs e)
    {
      ColorPickerDialog dialog;

      dialog = sender as ColorPickerDialog;

      if (dialog != null)
      {
        this.PreviewColor = dialog.Color;
      }
    }

    private Color _previewColor;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color PreviewColor
    { get { return _previewColor; }
      private set
      {
        if (_previewColor != value)
        {
          _previewColor = value;

          this.OnPreviewColorChanged(EventArgs.Empty); ;
        }
      }
    }

    /// <summary>
    /// Raises the <see cref="ColorChanged" /> event.
    /// </summary>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected virtual void OnColorChanged(EventArgs e)
    {
      EventHandler handler;

      handler = (EventHandler)this.Events[_eventColorChanged];

      handler?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="HelpRequest" /> event.
    /// </summary>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected virtual void OnHelpRequest(EventArgs e)
    {
      EventHandler handler;

      handler = (EventHandler)this.Events[_eventHelpRequest];

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

    #endregion

    #region Other

    private static EventHandlerList _eventHandlers;

    #endregion
  }
}
