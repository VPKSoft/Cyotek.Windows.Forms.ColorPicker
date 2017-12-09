using System;
using System.Drawing;
using System.Windows.Forms;
using FwColorDialog = System.Windows.Forms.ColorDialog;

namespace Cyotek.Windows.Forms.ColorPicker.Demo
{
  // Cyotek Color Picker controls library
  // Copyright © 2013-2017 Cyotek Ltd.
  // http://cyotek.com/blog/tag/colorpicker

  // Licensed under the MIT License. See license.txt for the full text.

  // If you use this code in your applications, donations or attribution are welcome

  internal partial class ColorPickerDialogDemoForm : BaseForm
  {
    #region Constructors

    public ColorPickerDialogDemoForm()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Methods

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      colorPreviewPanel.Color = Color.SeaGreen;
    }

    private void browseColorButton_Click(object sender, EventArgs e)
    {
      extendedColorDialog.Color = colorPreviewPanel.Color;
      extendedColorDialog.ShowAlphaChannel = showAlphaChannelCheckBox.Checked;

      if (extendedColorDialog.ShowDialog(this) == DialogResult.OK)
      {
        colorPreviewPanel.Color = extendedColorDialog.Color;
      }
    }

    private void closeToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    #endregion

    private void standardColorDialogButton_Click(object sender, EventArgs e)
    {
      standardColorDialog.Color = colorPreviewPanel.Color;

      if (standardColorDialog.ShowDialog(this) == DialogResult.OK)
      {
        colorPreviewPanel.Color = standardColorDialog.Color;
      }
    }

    private void extendedColorDialog_PreviewColor(object sender, EventArgs e)
    {
      dialogColorPreviewPanel.Color = extendedColorDialog.PreviewColor;
    }
  }
}
