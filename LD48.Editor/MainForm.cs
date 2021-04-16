namespace LD48.Editor
{
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class MainForm : Form
    {
        private readonly List<SceneViewer> _sceneViewers;

        public MainForm()
        {
            InitializeComponent();
            _sceneViewers = new List<SceneViewer>();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SpawnSceneViewer();
        }

        private void SpawnSceneViewer()
        {
            Task.Factory.StartNew(() =>
            {
                var sv = new SceneViewer();
                var c = Control.FromHandle(sv.Window.Handle);
                _sceneViewers.Add(sv);
                sv.Run();
                _sceneViewers.Remove(sv);
                sv.Dispose();
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var sv in _sceneViewers)
                sv.b += 10;
        }
    }
}
