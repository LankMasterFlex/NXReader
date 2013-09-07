using System;
using System.Drawing;
using System.Windows.Forms;
using reNX;
using reNX.NXProperties;

namespace NXReader
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    NXFile nx = new NXFile(openFileDialog.FileName);

                    TreeNode baseTreeNode = new TreeNode(openFileDialog.SafeFileName);
                    baseTreeNode.Tag = nx;
                    treeView.Nodes.Add(baseTreeNode);

                    IterateNodes(baseTreeNode, nx.BaseNode);
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Unable to load {0}{1}{1}{2}", openFileDialog.SafeFileName, Environment.NewLine, ex.ToString());
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var treeNode = treeView.SelectedNode;

            if (treeNode != null)
            {
                var nxFile = treeNode.Tag as NXFile;

                if (nxFile != null)
                {
                    treeView.Nodes.Remove(treeNode);
                    nxFile.Dispose();
                }
            }
        }

        private void IterateNodes(TreeNode parentTreeNode, NXNode parentNXNode)
        {
            foreach (NXNode nxNode in parentNXNode)
            {
                TreeNode treeNode = new TreeNode(nxNode.Name);
                treeNode.Tag = nxNode;

                parentTreeNode.Nodes.Add(treeNode);

                IterateNodes(treeNode, nxNode);
            }
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            NXNode node = e.Node.Tag as NXNode;
            pictureBoxCanvas.Image = null;

            if (node != null)
            {
                string info = string.Empty;

                if (node is NXValuedNode<Point>)
                {
                    info = node.ValueOrDie<Point>().ToString();
                }
                else if (node is NXValuedNode<string>)
                {
                    info = node.ValueOrDie<string>();
                }
                else if (node is NXValuedNode<double>)
                {
                    info = node.ValueOrDie<double>().ToString();
                }
                else if (node is NXValuedNode<long>)
                {
                    info = node.ValueOrDie<long>().ToString();
                }
                else if (node is NXBitmapNode)
                {
                    var bitmap = node.ValueOrDie<Bitmap>();
                    info = string.Concat("Width = ", bitmap.Width, ", Height = ", bitmap.Height);
                    pictureBoxCanvas.Image = bitmap;
                }

                textBoxInfo.Text = info;
            }

            toolStripStatusLabel.Text = string.Concat("Selection Type : ", e.Node.Tag.GetType().Name);
        }
    }
}
