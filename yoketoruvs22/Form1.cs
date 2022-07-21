using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace yoketoruvs22
{
    public partial class Form1 : Form
    {
        const bool isDebug = true;

        const int SpeedMax = 10;
        const int PlayerMax = 1;
        const int EnemyMax = 10;
        const int ItemMax = 19;
        int itemCount=0;
        const int ChrMax = PlayerMax + EnemyMax + ItemMax;
        Label[] chrs = new Label[ChrMax];
        int[] vx = new int[ChrMax];
        int[] vy = new int[ChrMax];
        const int PlayerIndex = 0;
        const int EnemyIndex = PlayerMax+PlayerMax;
        const int ItemIndex = EnemyMax+EnemyMax;
        const int StartTime = 100;

        const string PlayerText = "('ω')";
        const string EnemyText = "◆";
        const string ItemText = "★";

        static Random rand = new Random();

        int time = 0;

        enum State
        {
            None=-1,
            Title,
            Game,
            Gameover,
            Clear,
        }
        State currentState = State.None;
        State nextState = State.Title;

        [DllImport("user32.dll")]

        public static extern short GetAsyncKeyState(int vKey);

        public Form1()
        {
            InitializeComponent();

            for(int i = 0;i<ChrMax;i++)
            {
                chrs[i] = new Label();
                chrs[i].AutoSize = true;
                if(i == PlayerIndex)
                {
                    chrs[i].Text = PlayerText;
                }
                else if(i < ItemIndex)
                {
                    chrs[i].Text = EnemyText;
                }
                else
                {
                    chrs[i].Text = ItemText;
                }
                chrs[i].Font = tempLabel.Font;
                Controls.Add(chrs[i]);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {


            if (nextState != State.None)
            {
                initProc();
            }

            if(isDebug)
            {
                if(GetAsyncKeyState((int)Keys.O)<0)
                {
                    nextState = State.Gameover;
                }
                else if(GetAsyncKeyState((int)Keys.C) < 0)
                {
                    nextState = State.Clear;
                }
            }

            if(currentState==State.Game)
            {
                UpdateGame();
            }
        }

        void UpdateGame()
        {
            time--;
            timeLabel.Text = "Time" + time;

            Point mp = PointToClient(MousePosition);

            chrs[PlayerIndex].Left = mp.X - chrs[PlayerIndex].Width / 2;
            chrs[PlayerIndex].Top = mp.Y - chrs[PlayerIndex].Height / 2;
            //クリックしたところに移動
            //chrs[PlayerIndex].Left = Convert.ToString(Form1.X);
            //chrs[PlayerIndex].Top = Convert.ToString(Form1.Y);
            //chrs[PlayerIndex].Top += vy[i];
            for (int i =EnemyIndex;i<ChrMax;i++)
            {
                chrs[i].Left += vx[i];
                chrs[i].Top += vy[i];

                if(chrs[i].Left<0)
                {
                    vx[i] = Math.Abs(vx[i]);
                }
                if (chrs[i].Top < 0)
                {
                    vy[i] = Math.Abs(vy[i]);
                }
                if (chrs[i].Right > ClientSize.Width)
                {
                    vx[i] = -Math.Abs(vx[i]);
                }
                if (chrs[i].Bottom > ClientSize.Height)
                {
                    vy[i] = -Math.Abs(vy[i]);
                }

                if(   (mp.X >= chrs[i].Left)
                    &&(mp.X < chrs[i].Right)
                    &&(mp.Y >= chrs[i].Top)
                    &&(mp.Y < chrs[i].Bottom)
                    )
                {
                    if(i< ItemIndex)
                    {
                        nextState = State.Gameover;
                    }
                    else
                    {
                        //chrs[i].Visible = false;
                        itemCount--;
                        if(itemCount<=0)
                        {
                            nextState = State.Clear;
                        }
                        leftLabel.Text = $"★:{itemCount:00}";
                        vx[i] = 0;
                        vy[i] = 0;
                        chrs[i].Left = 10000;
                    }
                }
            }

            if (  (time<=0)
                &&(nextState ==State.None))
            {
                nextState = State.Gameover;
            }
        }

        void initProc()
        {
            currentState = nextState;
            nextState = State.None;

            switch (currentState)
            {
                case State.Title:
                    titleLabel.Visible=true;
                    startButton.Visible = true;
                    hiLabel.Visible = true;
                    copyrightLabel.Visible = true;
                    gameOverLabel.Visible = false;
                    clearLabel.Visible = false;
                    titleButton.Visible = false;
                    Saibutton.Visible = false;
                    break;

                case State.Game:
                    titleLabel.Visible = false;
                    hiLabel.Visible = false;
                    copyrightLabel.Visible = false;
                    startButton.Visible = false;
                    

                    for (int i =EnemyIndex;i<ChrMax;i++)
                    {
                        chrs[i].Left = rand.Next(ClientSize.Width - chrs[i].Width);
                        chrs[i].Top = rand.Next(ClientSize.Height - chrs[i].Height);
                        vx[i] = rand.Next(-SpeedMax, SpeedMax + 1);
                        vy[i] = rand.Next(-SpeedMax, SpeedMax + 1);
                    }

                    itemCount = ItemMax-9;
                    time = StartTime+1;

                    break;

                case State.Gameover:
                    gameOverLabel.Visible = true;
                    titleButton.Visible = true;
                    Saibutton.Visible = true;
                    break;

                case State.Clear:
                    clearLabel.Visible = true;
                    titleButton.Visible = true;
                    hiLabel.Visible = true;
                    break;

            }
        }

        private void clearLabel_Click(object sender, EventArgs e)
        {

        }

        private void startButton_Click(object sender, EventArgs e)
        {
            nextState = State.Game;
        }

        private void Saibutton_Click(object sender, EventArgs e)
        {
            nextState = State.Game;
            leftLabel.Text = ("★:10");
            gameOverLabel.Visible = false;
            titleButton.Visible = false;
            Saibutton.Visible = false;
        }

        private void titleButton_Click(object sender, EventArgs e)
        {
            nextState = State.Title;
        }
    }
}
