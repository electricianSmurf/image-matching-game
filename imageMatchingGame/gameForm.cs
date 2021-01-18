using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace imageMatchingGame
{
    public partial class gameForm : Form
    {
        List<PictureBox> pictureBoxList = new List<PictureBox>();

        List<Image> listDefaultImages;
        List<string> listDefaultImageNames;

        List<Image> listRandomImages;
        List<string> listRandomImageNames;

        PictureBox firstClickedPBox = null;
        PictureBox secondClickedPBox = null;

        bool isGameStarted;

        Random rnd = new Random();
        Image wallImage = Properties.Resources.wall;

        int PBoxWidth = 81, PBoxHeight = 68;

        public gameForm()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            createPBoxes();
        }
        private void createPBoxes()
        {
            for (int row = 0; row < 4; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    PictureBox pb = new PictureBox();
                    pb = createPBoxObject(pb);
                    pb.Location = calculatePBoxLocation(column, row);
                    pictureBoxList.Add(pb);
                    panel1.Controls.Add(pb);
                }
            }

            changeBGroundImagesToWall();
        }
        private PictureBox createPBoxObject(PictureBox PBox)
        {
            PBox.Size = new Size(PBoxWidth, PBoxHeight);
            PBox.SizeMode = PictureBoxSizeMode.StretchImage;
            PBox.BackgroundImageLayout = ImageLayout.Stretch;
            PBox.BackColor = Color.Silver;
            PBox.Tag = pictureBoxList.Count;
            PBox.Click += new EventHandler(pictureBox_Click);
            return PBox;
        }
        private Point calculatePBoxLocation(int column, int row)
        {
            //gaps are the gaps between picturebox rows and columns
            int gapX = PBoxWidth + 6;
            int gapY = PBoxHeight + 6;
            Point p = new Point(1 + (gapX * column), 1 + (gapY * row));
            return p;
        }
        private void changeBGroundImagesToWall()
        {
            for (int i = 0; i < pictureBoxList.Count; i++)
            {
                showImageOfPbox(pictureBoxList[i], false);// false means the image of pbox must be wall
            }
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Visible = false;
            pboxPressStart.Visible = false;
            addDefaultImages();
            addDefaultImageNames();

            createRandomImages();
            isGameStarted = true;
        }
        private void addDefaultImages()
        {
            listDefaultImages = new List<Image>()
            {
                Properties.Resources.key, Properties.Resources.car, Properties.Resources.baloon,
                Properties.Resources.computer, Properties.Resources.house, Properties.Resources.box,
                Properties.Resources.key, Properties.Resources.car, Properties.Resources.baloon,
                Properties.Resources.computer, Properties.Resources.house, Properties.Resources.box,
            };
        }
        private void addDefaultImageNames()
        {
            listDefaultImageNames = new List<string>()
            {
                "key", "car", "baloon", "computer", "house","box",
                "key", "car", "baloon", "computer", "house","box",
            };
        }
        private void createRandomImages()
        {
            listRandomImages = new List<Image>();
            listRandomImageNames = new List<string>();
            for (int i = 0; i < 12; i++)
            {
                int rndNumber = rnd.Next(listDefaultImages.Count);

                listRandomImages.Add(listDefaultImages[rndNumber]);
                listRandomImageNames.Add(listDefaultImageNames[rndNumber]);

                listDefaultImages.RemoveAt(rndNumber);
                listDefaultImageNames.RemoveAt(rndNumber);
            }
            listDefaultImages.TrimExcess();
            listDefaultImageNames.TrimExcess();
        }
        private void pictureBox_Click(object sender, EventArgs e)
        {
            PictureBox pboxClicked = sender as PictureBox;
            if (isGameStarted == true && pboxClicked.BackgroundImage != null)
            {
                showImageOfPbox(pboxClicked, true);//true means image must be shown
                if (firstClickedPBox == null)
                {
                    firstClickedPBox = pboxClicked;
                }
                else if (secondClickedPBox == null)
                {
                    secondClickedPBox = pboxClicked;
                    timer1.Enabled = true;
                }
            }
        }
        private void showImageOfPbox(PictureBox pbox, bool isShownOrHidden)
        {
            pbox.BackgroundImage = wallImage;
            pbox.Image = wallImage;
            if (isShownOrHidden)
            {
                pbox.Image = listRandomImages[Convert.ToInt32(pbox.Tag)];
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            imageControl();
            isGameStarted = gameFinished();
            if (!isGameStarted)
            {
                messageBoxForm mbForm = new messageBoxForm();
                mbForm.StartPosition = FormStartPosition.CenterParent;
                mbForm.ShowDialog(this);
                reloadForm();
            }
        }
        private void imageControl()
        {
            if (firstClickedPBox.Tag == secondClickedPBox.Tag)
            {
                secondClickedPBox = null;
            }
            else
            {
                int firstImageTag = Convert.ToInt32(firstClickedPBox.Tag);
                int secondImageTag = Convert.ToInt32(secondClickedPBox.Tag);
                if (listRandomImageNames[firstImageTag] == listRandomImageNames[secondImageTag])
                {
                    deleteTheSameImages(firstImageTag, secondImageTag);
                }
                else
                {
                    showImageOfPbox(firstClickedPBox, false);// hide images again
                    showImageOfPbox(secondClickedPBox, false);
                }
                firstClickedPBox = null;
                secondClickedPBox = null;
            }
        }
        private void deleteTheSameImages(int firstTag, int secondTag)
        {
            pictureBoxList[firstTag].BackgroundImage = null;
            pictureBoxList[secondTag].BackgroundImage = null;

            pictureBoxList[firstTag].Image = null;
            pictureBoxList[secondTag].Image = null;
        }
        private bool gameFinished()
        {
            for (int i = 0; i < pictureBoxList.Count; i++)
            {
                if (pictureBoxList[i].BackgroundImage != null)
                {
                    return true;
                }
            }
            return false;
        }
        private void reloadForm()
        {
            isGameStarted = false;
            btnStart.Visible = true;
            pboxPressStart.Visible = true;
            firstClickedPBox = null;
            secondClickedPBox = null;
            changeBGroundImagesToWall();

            clearImageInfoLists();
        }
        private void clearImageInfoLists()
        {
            listRandomImages.Clear();
            listRandomImageNames.Clear();

            listRandomImages.TrimExcess();
            listRandomImageNames.TrimExcess();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
