/**
 * This is simple proof-of-concept code for Opening and saving various filetypes,
 * feel free to improve upon and/or use these libraries in your own projects, assuming credit is
 * given where credit is due, the research into Models was done by MNL.
 * the idea behind using stream-based workers from aCynodonut, and
 * the writing of this library is of course done by myself.
 * 
 * OMI Stands for a new project I'm working on, calling it the OpenMinecraftInitiative, 
 * a unified collection of code meant as a working off point, something meant to be used 
 * to create new tools with the help of a new library, one that could be used to 
 * edit minecraft LCE.
 * 
 * That's all for now
 *      ~Phoenix
 */
#region Library References
using System;
using System.IO;
using System.Windows.Forms;

//Workers
using ColorWorker;
using LanguageWorker;
using MaterialWorker;
using ModelsWorker;
using UIWorker;
using ColorWorker.model;
using LanguageWorker.model;
using MaterialWorker.model;
using ModelsWorker.model;
using UIWorker.model;
#endregion

namespace OMI_Filetype_Library
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        #region variables
             //Containers
        ColorContainer ColorsContainer;
        LanguagesContainer LanguageContainer;
        MaterialContainer Materialscontainer;
        ModelContainer ModelsContainer;
        UIContainer UiContainer;
             //Parsers
        ColorParser cp = new ColorParser();
        LanguagesParser lp = new LanguagesParser();
        MaterialParser mp = new MaterialParser();
        ModelParser _mp = new ModelParser();
        UIParser up = new UIParser();
              //Builders
        ColorBuilder cb = new ColorBuilder();
        LanguageBuilder lb = new LanguageBuilder();
        //MaterialBuilder mb = new MaterialBuilder();
        //ModelBuilder _mb = new ModelBuilder();
        UIBuilder ub = new UIBuilder();

        #endregion

        #region Opening Files

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Models file|*.bin";
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                ModelsContainer = _mp.Parse(ofd.FileName);
            }
        } // models.bin Files

        private void button4_Click(object sender, EventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Localization file|*.loc";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LanguageContainer = lp.Parse(ofd.FileName);
            }
        } // Localization.loc files

        private void button6_Click(object sender, EventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Materials file|entityMaterials.bin";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Materialscontainer = mp.Parse(ofd.FileName);
            }
        } // entityMaterials.bin files

        private void button8_Click(object sender, EventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Colors file|*.col";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ColorsContainer = cp.Parse(ofd.FileName);
            }
        } // Colors Files

        private void button10_Click(object sender, EventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "FourJ UI file|*.fui";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                UiContainer = up.Parse(ofd.FileName);
            }
        } // FourJ UI Files

        #endregion

        #region Opening Files

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Models file|*.bin";
            if(sfd.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("WorkInProgress!");
            }
        } // models.bin Files

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Localization file|*.loc";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                lb.Build(LanguageContainer, sfd.FileName);
            }
        } // Localization.loc files

        private void button5_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Materials file|entityMaterials.bin";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("WorkInProgress!");
            }
        } // entityMaterials.bin files

        private void button7_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Colors file|*.col";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(sfd.FileName, cb.Build(ColorsContainer));
            }
        } // Colors Files

        private void button9_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "FourJ UI file|*.fui";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(sfd.FileName, ub.Build(UiContainer));
            }
        } // FourJ UI Files

        #endregion

    }
}
