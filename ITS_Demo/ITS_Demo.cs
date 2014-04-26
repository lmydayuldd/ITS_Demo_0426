using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ITS_Demo
{
    public partial class ITS_Demo : Form
    {
        const int amount = 50;
        const int cellwidth = 10;
        int Ped_num = 10;
        int tick = 0;
        
        int[] X = new int[10] { 1, 2, 1, 2, 1, 2, 1, 2, 1, 2 };
        int[] Y = new int[10] { 15, 16, 18, 19, 22, 21, 31, 32, 34, 35 };
        //System.Drawing.Graphics myGraphics;
        System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.SlateBlue);
        System.Drawing.Pen myBlackPen = new System.Drawing.Pen(System.Drawing.Color.Black);
        //SolidBrush redBrush = new SolidBrush(Color.Red);
        //SolidBrush exitBrush = new SolidBrush(Color.FromArgb(50, 255, 50));

        Cell[,] Position = new Cell[amount, amount];
        public ITS_Demo()
        {
            InitializeComponent();
            
        }

        public void main()
        {
            for (int aa = 0; aa < amount; aa++)//对象初始化
            {
                for (int bb = 0; bb < amount; bb++)
                {
                    Position[aa, bb] = new Cell(0, false);
                }

            }


            //定义静态位势
            int Exit_row = 3;

            //边框
            for (int a = 0; a < amount; a++)
            {

                Position[a, 0].staticfloorfield = -1;
                Position[0, a].staticfloorfield = -1;
                Position[a, amount-1].staticfloorfield = -1;
                Position[amount-1, a].staticfloorfield = -1;


            }
            //出口
            Position[Exit_row, 0].staticfloorfield = Position[Exit_row + 1, 0].staticfloorfield = Position[Exit_row + 2, 0].staticfloorfield = 0;
            //内部位势
            for (int b = 1; b < amount-1; b++)
            {
                //出口平行
                Position[Exit_row, b].staticfloorfield = Position[Exit_row + 1, b].staticfloorfield = Position[Exit_row + 2, b].staticfloorfield = b;

                //出口上部
                int c = 1;
                while (c < Exit_row)
                {
                    Position[c, b].staticfloorfield = Math.Abs(c - Exit_row) + Math.Abs(b - 0);
                    c++;
                }

                //出口下部
                int d = Exit_row + 3;
                while (d < 49)
                {
                    Position[d, b].staticfloorfield = Math.Abs(d - Exit_row) + Math.Abs(b - 0);
                    d++;
                }

            }

            //计算行人周围静态位势最小的点
            int[] MinFFPostion = new int[10];//10个行人每个行人附近最小静态位势点的序号
            for (int e = 0; e < 10; e++)
            {

                int[] NeighbourCell = new int[9]{//行人附近（包括行人）共九点的位势数组，数组和坐标有差异，坐标换到数组，要转置
                                                  Position[Y[e]-1,X[e]-1].staticfloorfield,//0、左上
                                                  Position[Y[e]-1,X[e]].staticfloorfield,  //1、正上                
                                                  Position[Y[e]-1,X[e]+1].staticfloorfield,//2、右上
                                                  Position[Y[e],X[e]+1].staticfloorfield,  //3、正右
                                                  Position[Y[e]+1,X[e]+1].staticfloorfield,//4、右下
                                                  Position[Y[e]+1,X[e]].staticfloorfield,  //5、正下
                                                  Position[Y[e]+1,X[e]-1].staticfloorfield,//6、左下
                                                  Position[Y[e],X[e]-1].staticfloorfield,  //7、正左
                                                  Position[Y[e],X[e]].staticfloorfield,    //8、初始位置
                                                };
                int ii = 0;
                while (ii < 9)//如果邻居是障碍的话，则值置为无穷大
                {
                    if (NeighbourCell[ii] == -1)
                    {
                        NeighbourCell[ii] = int.MaxValue;
                    }
                    ii++;
                }
                MinFFPostion[e] = Min(NeighbourCell);//返回邻居中最小值的序号到数组中

            }


        }

        public int Min(int[] array)//计算数组最小值
        {

            int a = 0;
            int b = array[0];
            for (int i = 0; i < array.Length; i++)
            {
                a = array[i] < b ? (b = array[i]) : b; //一轮for循环求出最小值
            }
            int c = Array.IndexOf(array, a);
            //int ii = 0;
            //while(array[ii]!=a)
            //{
            //    ii++;
            //}
            //return ii;
            return c;

        }

        private void Quit_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void Start_Click(object sender, EventArgs e)
        {
           
            int[] X = new int[10]{ 1, 2, 1, 2, 1, 2, 1, 2, 1, 2};
            int[] Y = new int[10]{ 15,16,18,19,22,21,31,32,34,35};
            FileStream myFS = new FileStream(@"Demo.txt", FileMode.Open, FileAccess.Read);
            StreamReader mySR=new StreamReader(myFS);
            //int str = mySR.Read();
            String str = mySR.ReadToEnd();
            string[] move_str = str.Split(new char[] { '\n', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int[] move_array = new int[move_str.Length];   //用来存放将字符串转换成int[] 
            for (int i = 0; i < move_str.Length; i++)
            {
                move_array[i] = Convert.ToInt16(move_str[i]);
            }
            Stopwatch sw = new Stopwatch();//计时
            sw.Start();
            int counter = 0;
	        while(move_array!=null)
	        {
                int i = counter % 10;
		        
		        if(move_array[counter]==6) break;//结束判断

		        int x = X[i];
		        int y = Y[i];
		
		        //删除
		        UnDrawPed(x,y);
		        //添加
                
		        switch(move_array[counter])
		        {
		        case 0:DrawPedDirection(x,y,0);break;//stay
		        case 1:y--;DrawPedDirection(x,y,1);break;//up
		        case 2:y++;DrawPedDirection(x,y,2);break;//down
		        case 3:x--;DrawPedDirection(x,y,3);break;//left
		        case 4:x++;DrawPedDirection(x,y,4);break;//right
		        }
		        X[i] = x;
		        Y[i] = y;
                
                
		        counter++;
                if (i == 9) System.Threading.Thread.Sleep(30); 
	        }

            mySR.Close();
            mySR.Dispose();
	        myFS.Close();
            myFS.Dispose();
	        MessageBox.Show("运行完毕！");
            sw.Stop();
            Time.Text = sw.ElapsedMilliseconds.ToString();

            //Pen myPen = new Pen(Color.Black, 3);
            //Rectangle myRectangle = new Rectangle(100, 50, 80, 40);
            //System.Drawing.Graphics myGraphics;
            //myGraphics = this.CreateGraphics();
            //myGraphics.DrawRectangle(myPen, myRectangle);


        }

        private void ITS_Demo_Load(object sender, EventArgs e)
        {
            //MessageBox.Show("Form Loaded!");
         
        }

        private void ITS_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            /*//Pen myPen = new Pen(Color.Black, 3);
            //Rectangle myRectangle = new Rectangle(100, 50, 80, 40);
            //System.Drawing.Graphics myGraphics;
       
            Graphics p = e.Graphics;
            int x=10;
            int y=10;

            //Graphics myGraphics = e.Graphics;
            //System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.White);
            //System.Drawing.Pen myBlackPen = new System.Drawing.Pen(System.Drawing.Color.Black);
            //System.Drawing.Pen myObstaclePen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(255,50,50));
            //SolidBrush redBrush = new SolidBrush(Color.Red);
            //SolidBrush exitBrush = new SolidBrush(Color.FromArgb(50,255,50));
            //System.Drawing.Graphics myGraphics;
            //myGraphics = this.CreateGraphics();

            for(int i = 1; i < amount; i++)
	        {
		        p.DrawLine(myPen,x + cellwidth * i,y,x + cellwidth * i,y + cellwidth * amount);
	        }
	        for(int i = 1; i < amount; i++)
	        {
                p.DrawLine(myPen,x, y + cellwidth * i,x + cellwidth * amount,y + cellwidth * i);
	        }//白色间隔线

            p.DrawRectangle(myBlackPen,10, 10, 500, 500);//黑色边框

            //for (int i = 0; i < 40; i++)//障碍物
            //{
            //    DrawObstacle(i, 9);
            //    DrawObstacle(i, 40);

            //    //myGraphics.DrawRectangle(myObstaclePen,10+i*10+1,10+9*10+1,10,10);
            //    //myGraphics.FillRectangle(redBrush, 10 + i * 10 + 1, 10 + 9 * 10 + 1, 9, 9);
            //    //myGraphics.FillRectangle(redBrush, 10 + i * 10 + 1, 10 + 40 * 10 + 1, 9, 9);
            //}
            for (int i = 0, a = 3; i < 3; i++)//出口
            {
                
                DrawExit(0, a + i);
                //if (i == 2) a = 41;

                //myGraphics.FillRectangle(exitBrush, 11, 10 + (a+i) * 10 + 1, 9, 9);
            }

            DrawPedDirection(1, 15, 0);//行人
            DrawPedDirection(2, 16, 0);
            DrawPedDirection(1, 18, 0);
            DrawPedDirection(2, 19, 0);
            DrawPedDirection(1, 22, 0);
            DrawPedDirection(2, 21, 0);
            DrawPedDirection(1, 31, 0);
            DrawPedDirection(2, 32, 0);
            DrawPedDirection(1, 34, 0);
            DrawPedDirection(2, 35, 0);

            int Ped_num = 10;//随机行人形成
            int[] ran_x = new int[Ped_num];
            int[] ran_y = new int[Ped_num];
            int tick = int.Parse(textBox1.Text);//此处可以改变tick的输入，使随机数有一个确定的种子，使得每次产生的点都不同
            Random ran_ped = new Random(tick);
            for (int i = 0; i < Ped_num; i++)
            {
                ran_x[i] = ran_ped.Next(1, 49);
                ran_y[i] = ran_ped.Next(1, 49);
                DrawPedDirection(ran_x[i], ran_y[i], 0);
            }

            //p.Dispose();*/

        }



        public void DrawRect(int x1,int y1, int width, int height, int r, int g, int b)
        {//画方块
            Graphics p = this.CreateGraphics();
            Color customColor = Color.FromArgb(r,g,b);
            SolidBrush Brush = new SolidBrush(customColor);
            p.FillRectangle(Brush, x1, y1, width, height);
            p.Dispose();
          
        }


        public void DrawObstacle(int column, int row)
        {//画障碍物
            int x = 10 + column * cellwidth;
            int y = 10 + row * cellwidth;
            DrawRect(x + 1, y + 1, 9, 9, 255, 50, 50);
        }

        public void DrawExit(int column, int row)
        {//画出口
            int x = 10 + column * cellwidth;
            int y = 10 + row * cellwidth;
            DrawRect(x + 1, y + 1, 9, 9, 50, 255, 50);
        }
        //public void DrawPed(int column, int row)
        //{//画行人，没有用，在DrawPedDirection中包括此步了
        //    int x = 10 + column * cellwidth;
        //    int y = 10 + row * cellwidth;
        //    DrawRect(x + 2, y + 2, 9, 9, 0, 0, 255);
        // }
        public void UnDrawPed(int column, int row)
        {//消行人
	        int x = 10 + column * cellwidth;
	        int y = 10 + row * cellwidth;
            DrawRect(x + 1, y + 1, 9, 9, 240, 240, 240);
        }
        public void DrawPedDirection(int column, int row, int mod)
        {//画行人带方向(行人初始位置及方向)
	        int x = 10 + column * cellwidth;
	        int y = 10 + row * cellwidth;
	        DrawRect(x+1,y+1,9,9,0,0,255);
	        switch (mod)
	        {
            case 0: DrawRect(x + 4, y + 4, 3, 3, 0, 255, 0); break;//stay，两条线中间是9*9的方格，选中间3*3的方格作为方向指示
            case 1: DrawRect(x + 4, y + 1, 3, 3, 0, 255, 0); break;//up
            case 2: DrawRect(x + 4, y + 7, 3, 3, 0, 255, 0); break;//down
            case 3: DrawRect(x + 1, y + 4, 3, 3, 0, 255, 0); break;//left
            case 4: DrawRect(x + 7, y + 4, 3, 3, 0, 255, 0); break;//right
	        }
        }

  
        private void Initial_Click(object sender, EventArgs e)
        {
            
            System.Drawing.Graphics myGraphics;
            myGraphics = this.CreateGraphics();

            for (int i = 0; i < amount; i++)
            {
                for (int j = 0; j < amount;j++ )
                    UnDrawPed(i, j);
            }


            int x = 10;
            int y = 10;

            for (int i = 1; i < amount; i++)
            {
                myGraphics.DrawLine(myPen, x + cellwidth * i, y, x + cellwidth * i, y + cellwidth * amount);
            }
            for (int i = 1; i < amount; i++)
            {
                myGraphics.DrawLine(myPen, x, y + cellwidth * i, x + cellwidth * amount, y + cellwidth * i);
            }//白色间隔线

            myGraphics.DrawRectangle(myBlackPen, 10, 10, 500, 500);//黑色边框

           
            for (int i = 0, a = 3; i < 3; i++)//出口
            {

                DrawExit(0, a + i);
                //if (i == 2) a = 41;

                //myGraphics.FillRectangle(exitBrush, 11, 10 + (a+i) * 10 + 1, 9, 9);
            }

            DrawPedDirection(1, 15, 0);//行人
            DrawPedDirection(2, 16, 0);
            DrawPedDirection(1, 18, 0);
            DrawPedDirection(2, 19, 0);
            DrawPedDirection(1, 22, 0);
            DrawPedDirection(2, 21, 0);
            DrawPedDirection(1, 31, 0);
            DrawPedDirection(2, 32, 0);
            DrawPedDirection(1, 34, 0);
            DrawPedDirection(2, 35, 0);
            int[] ran_x = new int[Ped_num];
            int[] ran_y = new int[Ped_num];
            Random ran_ped = new Random(tick);
            if (Random.Text.Trim()==string.Empty)//随机行人形成
            {
                MessageBox.Show("请输入随机数");//弹出消息窗口
            }
            else
            {
                tick = int.Parse(Random.Text);//此处可以改变tick的输入，使随机数有一个确定的种子，使得每次产生的点都不同
                for (int i = 0; i < Ped_num; i++)
                {
                    ran_x[i] = ran_ped.Next(1, amount-1);
                    ran_y[i] = ran_ped.Next(1, amount-1);
                    DrawPedDirection(ran_x[i], ran_y[i], 0);
                }
            }
            myGraphics.Dispose();
        }

        private void Random_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

   
    }

    public class Cell : Form
    {
        public int staticfloorfield;
        public bool empty;


        public Cell(int staticfloorfield, bool empty)
        {

            this.staticfloorfield = staticfloorfield;
            empty = true;
        }
    }
}
