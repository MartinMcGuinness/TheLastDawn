using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Editor.Elements;
using System.IO;

namespace Editor.Editor1
{
    public partial class FormProperties : Form
    {
        public String NewElementType;
        public Game Game;
        private Scene scene;

        // This variable is used to disable events while reseting and initializing controls.
        private Element selectionEvents;

        private Element selection;
        public Element Selection
        {
            get { return selection; }
            set 
            {
                selection = value;
                selectionEvents = null;
                flowLayoutPanel1.Visible = (selection != null);

                if (flowLayoutPanel1.Visible)
                    listBoxAvailableElements.Size = initialSize;
                else
                    listBoxAvailableElements.Size = new Size(initialSize.Width, initialSize.Height + flowLayoutPanel1.Size.Height + 30);

                reset();
                if(selection != null)
                    showElementInForm();

                selectionEvents = selection;
            }
        }

        private void showElementInForm()
        {
            setInitialValues();


            if (selection is IActivable)
            {
                flowLayoutPanelActive.Visible = true;
                checkBoxActive.Checked = ((IActivable)selection).Active;
            }

            if (selection is Editor.Elements.Activator)
            {
                flowLayoutPanelActivableElementId.Visible = true;
                textBoxActivableElementId.Text = ((Editor.Elements.Activator)selection).ActivableElementId;
            }

            //ToDo
        }

        private void reset()
        {
            labelElementType.Text = "(no selection)";
            textBoxId.Text = "";
            numericUpDownPositionX.Value = 0;
            numericUpDownPositionY.Value = 0;
            numericUpDownWidth.Value = numericUpDownWidth.Minimum;
            numericUpDownHeight.Value = numericUpDownHeight.Minimum;
            numericUpDownRotation.Value = 0;

            //visibility
            flowLayoutPanelActivableElementId.Visible = false;
            flowLayoutPanelActive.Visible = false;
            flowLayoutPanelFinalPosition.Visible = false;
            flowLayoutPanelInitialPosition.Visible = false;
            flowLayoutPanelOtherSocketId.Visible = false;
            flowLayoutPanelScale.Visible = false;
            flowLayoutPanelSpeed.Visible = false;
            flowLayoutPanelStepsNumber.Visible = false;
            flowLayoutPanelLinksNumber.Visible = false;
            flowLayoutPanelRotorsNumber.Visible = false;
            flowLayoutPanelFixedRotation.Visible = false;
            flowLayoutPanelLinkWidth.Visible = false;
            flowLayoutPanelLinkHeight.Visible = false;
            flowLayoutPanelAngularSpeed.Visible = false;
            flowLayoutPanelTextureName.Visible = false;
            flowLayoutPanelNextLevel.Visible = false;
            flowLayoutPanelAlsoEnergy.Visible = false;
            flowLayoutPanelSoundName.Visible = false;
            flowLayoutPanelOtherTubeId.Visible = false;
            flowLayoutPanelVolume.Visible = false;
            flowLayoutPanelAcceleration.Visible = false;
            flowLayoutPanelVelocity.Visible = false;
            flowLayoutPanelColor.Visible = false;
            flowLayoutPanelScaleTarget.Visible = false;
            flowLayoutPanelLeftShift.Visible = false;
            flowLayoutPanelRightShift.Visible = false;
            flowLayoutPanelUpShift.Visible = false;
            flowLayoutPanelDownShift.Visible = false;
            flowLayoutPanelText.Visible = false;
            flowLayoutPanelCratesNumber.Visible = false;
            flowLayoutPanelBring.Visible = false;
        }

        private void setInitialValues()
        {
            //values for every element
            labelElementType.Text = selection.GetType().Name;
            textBoxId.Text = selection.Id;
            numericUpDownPositionX.Value = (decimal)selection.Position.X;
            numericUpDownPositionY.Value = (decimal)selection.Position.Y;
            numericUpDownWidth.Value = (decimal)selection.Width;
            numericUpDownHeight.Value = (decimal)selection.Height;
            numericUpDownRotation.Value = (decimal)selection.Rotation;
        }

        private Size initialSize;

        public FormProperties(Game game, Scene scene)
        {
            this.Game = game;
            this.scene = scene;
            InitializeComponent();
            Top = 0;
            Left = Screen.PrimaryScreen.WorkingArea.Size.Width - Size.Width;
            Size = new Size(Size.Width, Screen.PrimaryScreen.WorkingArea.Size.Height);
            checkBoxShowDebug.Checked = scene.PhysicsDebug.Enabled;
            checkBoxPhysicsEngine.Checked = scene.World.Enabled;
            checkBoxShowEmblems.Checked = SelectionManager.ShowEmblems;
            flowLayoutPanel1.Visible = false;
            initialSize = listBoxAvailableElements.Size;
            listBoxAvailableElements.Size = new Size(initialSize.Width, initialSize.Height + flowLayoutPanel1.Size.Height + 30);

            //foreach (string i in Directory.GetFiles(@"Content\backgrounds\"))
            //    comboBoxTextureName.Items.Add(Path.GetFileNameWithoutExtension(i));
        }

        private void textBoxId_TextChanged(object sender, EventArgs e)
        {
            if (selectionEvents != null)
                selectionEvents.Id = textBoxId.Text;
        }

        private void numericUpDownPositionX_ValueChanged(object sender, EventArgs e)
        {
            if (selectionEvents != null)
                selectionEvents.Position = new Vector2((float)numericUpDownPositionX.Value, selectionEvents.Position.Y);
        }

        private void numericUpDownPositionY_ValueChanged(object sender, EventArgs e)
        {
            if (selectionEvents != null)
                selectionEvents.Position = new Vector2(selectionEvents.Position.X, (float)numericUpDownPositionY.Value);
        }

        private void numericUpDownWidth_ValueChanged(object sender, EventArgs e)
        {
            if (selectionEvents != null)
                selectionEvents.Width = (float)numericUpDownWidth.Value;
        }

        private void numericUpDownHeight_ValueChanged(object sender, EventArgs e)
        {
            if (selectionEvents != null)
                selectionEvents.Height = (float)numericUpDownHeight.Value;
        }

        private void numericUpDownRotation_ValueChanged(object sender, EventArgs e)
        {
            if (selectionEvents != null)
                selectionEvents.Rotation = (float)numericUpDownRotation.Value;
        }

        

        private void checkBoxActive_CheckedChanged(object sender, EventArgs e)
        {
            if (selectionEvents != null)
                ((IActivable)selectionEvents).Active = checkBoxActive.Checked;
        }

        private void textBoxActivableElementId_TextChanged(object sender, EventArgs e)
        {
            if (selectionEvents != null)
                ((Editor.Elements.Activator)selectionEvents).ActivableElementId = textBoxActivableElementId.Text;
        }

        private void checkBoxShowDebug_CheckedChanged(object sender, EventArgs e)
        {
            scene.PhysicsDebug.Enabled = checkBoxShowDebug.Checked;
        }

        private void checkBoxPhysicsEngine_CheckedChanged(object sender, EventArgs e)
        {
            scene.World.Enabled = checkBoxPhysicsEngine.Checked;
        }

        private void listBoxAvailableElements_SelectedValueChanged(object sender, EventArgs e)
        {
            NewElementType = listBoxAvailableElements.Text;
        }

       

        private void checkBoxShowEmblems_CheckedChanged(object sender, EventArgs e)
        {
            SelectionManager.ShowEmblems = checkBoxShowEmblems.Checked;
        }

        

        //private void textBoxNextLevel_TextChanged(object sender, EventArgs e)
        //{
        //    if (selectionEvents != null)
        //        ((Endpoint)selectionEvents).NextLevel = textBoxNextLevel.Text;
        //}


        //private void textBoxSoundName_TextChanged(object sender, EventArgs e)
        //{
        //    if (selectionEvents != null)
        //        ((Sound)selectionEvents).SoundName = textBoxSoundName.Text;
        //}

        //private void numericUpDownVolume_ValueChanged(object sender, EventArgs e)
        //{
        //    if (selectionEvents != null)
        //        ((Sound)selectionEvents).Volume = (float)numericUpDownVolume.Value;
        //}


        //private void numericUpDownScaleTarget_ValueChanged(object sender, EventArgs e)
        //{
        //    if (selectionEvents != null)
        //        ((CameraScale)selectionEvents).ScaleTarget = (float)numericUpDownScaleTarget.Value;
        //}

        //private void textBoxText_TextChanged(object sender, EventArgs e)
        //{
        //    if (selectionEvents != null && selectionEvents is Hint)
        //        ((Hint)selectionEvents).Text = textBoxText.Text;
        //    else if (selectionEvents != null && selectionEvents is StoryTelling)
        //        ((StoryTelling)selectionEvents).Text = textBoxText.Text;
        //}


        //private void checkBoxAlsoEnergy_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (selectionEvents as Endpoint != null)
        //        ((Endpoint)selectionEvents).AlsoEnergy = checkBoxAlsoEnergy.Checked;
        //}

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            string header = "How to work with the Editor:\n\n";
            string message = "Ctrl + O  -  Open an xml file with a level.\n" +
                "LeftCtrl + S  -  Save your currently built level to xml.\n" +
                "LeftCtrl + N  -  Clean the currently built level.\n" +
                "Left mouse click  -  Select an element.\n" +
                "Right mouse click  -  Add a selected element from the list to the level.\n" +
                "W/A/S/D  -  Move the selected element up/left/down/right.\n" +
                "LeftShift + W/A/S/D  -  Move the selected element faster.\n" +
                "[+]/[-]  -  Change the width of the selected object (if possible).\n" +
                "LeftShift + [+]/[-]  -  Change the width of the selected object faster.\n" +
                "LeftCtrl + [+]/[-]  -  Change the height of the selected object.\n" +
                "LeftShift + LeftCtrl + [+]/[-]  -  Change the height of the selected object faster.\n";

            MessageBox.Show(header + message);
        }
    }
}
