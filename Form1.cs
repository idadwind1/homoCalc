using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Media;

namespace homoCalc
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent(); 
            CheckForIllegalCrossThreadCalls = false;
        }

        public List<int> ParesStringList(List<string> strlist)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < strlist.Count; i++)
                result.Add(int.Parse(strlist[i]));
            return result;
        }

        public List<string> SplitStringIntoGroup(int LettersInGroup, string str)
        {
            List<string> result = Regex.Split(str, "(?<=\\G.{" + LettersInGroup + "})").ToList();
            result.Remove("");
            return result;
        }

        public string[] Homo(double number, bool flag = false)
        {
            checkBox2.Enabled = false;
            toolStripStatusLabel1.Text = "请稍后...我已经迫不及待力";
            List<string> result = new List<string>();
            if (!checkBox2.Checked || flag)
            {
                string resultS = "";
                double resultNum;
                /*
                Thread thread = new Thread(() =>
                {*/
                List<List<int>> p = new List<List<int>>();//数的组合
                /*
                * 1. 1,1,4,5,1,4
                * 2. 11,45,14
                * 3. 114,514
                * 4. 1145,14
                * 5. 11451,4
                */
                /*try
                {*/
                if (number == 114514) { result.Add("114514"); }
                for (int i = 0; i < "114514".Length - 1; i++)
                {
                    List<int> str = ParesStringList(SplitStringIntoGroup(i + 1, "114514"));
                    p.Add(str);
                    if (checkBox1.Checked) toolStripStatusLabel1.Text = "基础数字组合列表" + i + ": " + string.Join(",", str.ToArray());
                }
                int pCount = p.Count;
                for (int i = 0; i < pCount; i++)
                {
                    for (int j = 0; j < pCount; j++)
                    {
                        List<int> ints = new List<int>();
                        ints.AddRange(p[i]);
                        ints.AddRange(p[j]);
                        if (checkBox1.Checked) toolStripStatusLabel1.Text = "拼接数字组合列表" + i * j + ": " + string.Join(",", ints.ToArray());
                        p.Add(ints);
                    }
                }
                List<int> ops = new List<int>();//每个数之间的运算符    0:+,1:-,2:*,3:/
                List<int> numsSgn = new List<int>();//每个数的正负    0:+,1:-
                for (int i = 0; i < p.Count(); i++)//数的组合  p[i] = { 1,1,4,5,1,4 }
                {
                    ops.Clear();
                    for (int a = 0; a < p[i].Count() - 1; a++) ops.Add(0);//初始化ops
                    numsSgn.Clear();
                    for (int a = 0; a < p[i].Count(); a++) numsSgn.Add(0);//初始化numsSgn
                    for (int j = 0; j < Math.Pow(p[i].Count() - 1, 4); j++)//j: 所有符号组合可能  j = 0
                    {
                        resultS = "";
                        for (int n = 1; n < p[i].Count() - 1; n++) resultS += "(";
                        resultS += p[i][0].ToString();
                        resultNum = p[i][0];
                        switch (numsSgn[0])
                        {
                            case 0:
                                resultNum = Math.Abs(resultNum);
                                break;
                            case 1:
                                resultNum = Math.Abs(resultNum) * -1;
                                break;
                        }
                        for (int k = 1; k < p[i].Count(); k++)//k: k: 遍历p[i]中的每个数的下标，用作符号和p[i]中的数组合  k = 0
                        {
                            double num = p[i][k];//当前遍历到的数
                            string numS = num.ToString();
                            switch (numsSgn[k])
                            {
                                case 0:
                                    num = Math.Abs(num);
                                    numS = "(-" + numS + ")";
                                    break;
                                case 1:
                                    num = Math.Abs(num)*-1;
                                    numS = numS.Replace('-',char.MinValue);
                                    break;
                            }
                            switch (ops[k - 1])
                            {
                                case 0:
                                    resultS += " + " + numS + ")";
                                    resultNum += num;
                                    break;
                                case 1:
                                    resultS += " - " + numS + ")";
                                    resultNum -= num;
                                    break;
                                case 2:
                                    resultS += " * " + numS + ")";
                                    resultNum *= num;
                                    break;
                                case 3:
                                    resultS += " / " + numS + ")";
                                    resultNum /= num;
                                    break;
                            }
                            if (resultNum == number && !result.Contains(resultS))
                                result.Add(resultS);
                        }
                        ops[0]++;
                        numsSgn[0]++;
                        for (int l = 0; l < ops.Count; l++)
                        {
                            if (l == ops.Count - 1) break;
                            if (ops[l] > 3) { ops[l] -= 3; ops[l + 1]++; }
                        }
                        for (int l = 0; l < numsSgn.Count; l++)
                        {
                            if (l == numsSgn.Count - 1) break;
                            if (numsSgn[l] > 1) { ops[l] -= 1; ops[l + 1]++; }
                        }
                        if (checkBox1.Checked) toolStripStatusLabel1.Text = "i < " + p.Count() + ", j < " + Math.Pow(p[i].Count() - 1, 4) + ", k < " + (p[i].Count() - 1) + ". " +
                        "i = " + i + ", j = " + j +
                        ". result: " + resultS + "=" + resultNum;
                    }
                }
                if (result.Count == 0 && !flag)
                {
                    throw new Exception("未搜索到算式啊啊啊啊啊，可以试试深度搜索");
                }
                /*}
                catch(Exception ex)
                {
                    toolStripStatusLabel1.Text = "出现了一个一个一个错误（悲）: 你是一个一个一个“" + ex.Message + "”，这么臭的" + number + "还有必要求证吗";
                }*/
                /*});
                thread.Start();
                thread.Join();*/
            }
            if ((checkBox2.Checked && !flag) || (result.Count == 0 && flag))
            {
                //深度搜索，随机生成一个较小的数字，为它生成一个式子，和另一个较小的数字的式子相加减。算法中较小的数字可以在-number/2到number/2中选取
                Random random = new Random();
                int loop = (int)numericUpDown1.Value;
                if (checkBox1.Checked) toolStripStatusLabel1.Text = "未找到符合算式，启动B方案，循环持续" + loop + "次";
                for (int a = 0; a < loop; a++)//生成多少算式
                {
                    double num1;
                    try
                    {
                        num1 = random.Next((int)number / -2, (int)number / 2);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        num1 = random.Next((int)number / 2, (int)number / -2);
                    }
                    double num2 = 0;
                    if (num1 >= 0)
                        num2 = number - num1;
                    else
                        num2 = number + num1;
                    if (checkBox1.Checked) toolStripStatusLabel1.Text = "a < " + loop + ", a = " + a +
                            ", num1 = " + num1 + ", num2 = " + num2;
                    string[] strls1 = Homo(num1, true);
                    string[] strls2 = Homo(num2, true);
                    if (num1 < 0) result.Add("(" + strls1[random.Next(strls1.Length - 1)] + ") + (" + strls2[random.Next(strls2.Length - 1)] + ")");
                    else result.Add("(" + strls1[random.Next(strls1.Length - 1)] + ") - (" + strls2[random.Next(strls2.Length - 1)] + ")");
                }
            }
            if (flag)
                return result.ToArray();
            toolStripStatusLabel1.Text = "完成力（喜）";
            SoundPlayer sp = new SoundPlayer();
            sp.Stream = Properties.Resources.erro;
            sp.Play();
            return result.ToArray();
        } 

        private void button1_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                string result = "";
                try
                {
                    string[] homo = Homo(double.Parse(textBox1.Text));
                    for (int i = 0; i < homo.Length; i++)
                    {
                        //result += double.Parse(textBox1.Text).ToString() + " = " + simpform(homo[i]) + "\n";
                        result += double.Parse(textBox1.Text).ToString() + " = " + homo[i] + "\n";
                    }
                }
                catch (Exception ex)
                {
                    SoundPlayer sp = new SoundPlayer();
                    sp.Stream = Properties.Resources.succ;
                    sp.Play();
                    toolStripStatusLabel1.Text = "出现了一个一个一个错误: 一个一个一个“" + ex.Message.Replace('\n',',').Replace('\r',char.MinValue) + "”，这么臭的" + textBox1.Text + "还有必要求证吗";
                }
                checkBox2.Enabled = true;
                richTextBox1.Text = result;
            });
            thread.Name = "Homo特有的线程";
            thread.Start();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            textBox1.Size = button1.Size = new Size(Size.Width - 34, 21);
            richTextBox1.Width = button1.Width;
            richTextBox1.Height = ClientRectangle.Height - 100;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //MessageBox.Show(simpform("((-1) + (-1)) + (-1)"));
            Form1_SizeChanged(sender, e);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "恶臭论证器\n" +
                "灵感来源于GitHub,itorr/homo（点击帮助按钮跳转），但未使用其代码\n" +
                "这么臭的论证器有必要存在吗？\n" +
                "本软件没有括号简化和符号简化算法，需要自行去括号，敬请谅解",
                "关于",
                MessageBoxButtons.OK,
                MessageBoxIcon.None,
                MessageBoxDefaultButton.Button1,
                0,
                "https://https://github.com/itorr/homo"
            );
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter) button1_Click(sender, e);
                else if (e.KeyCode == Keys.Up) { textBox1.Text = (double.Parse(textBox1.Text) + 1).ToString(); e.Handled = true; }
                else if (e.KeyCode == Keys.Down) { textBox1.Text = (double.Parse(textBox1.Text) - 1).ToString(); e.Handled = true; }
            }
            catch (FormatException) { }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = checkBox2.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
                toolStripStatusLabel1.Text = "就绪";
        }

        public string simpform(string formula)
        {
            /*
             * (1 + 1) + 1 => 1 + 1 + 1
             * 1 + (1 + 1) => 1 + 1 + 1
             * 
             */
            string result = formula;
            result = Regex.Replace(result, "^\\((\\(?-?\\d*\\)?)\\s\\+\\s(\\(?-?\\d*\\)?)\\)\\s\\+\\s(\\(?-?\\d*\\)?)", "$1 + $2 + $3");
            return result;
        }
    }
}
