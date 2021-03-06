﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Шифрование_и_Дешифрование
{
    public partial class Form1 : Form
    {
        string [] alphabet = {"АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ ,.!?-:;_()1234567890", "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ ,.!?-:;_()",    //
                                            "ABCDEFGHIJKLMNOPQRSTUVWXYZ ,.!?-:;_()1234567890", "ABCDEFGHIJKLMNOPQRSTUVWXYZ ,.!?-:;_()" }; //Массив алфавитов
        string [] mask = {"[а-яёА-Яё]", "[а-яёА-Яё]", "[a-zA-Z]", "[a-zA-Z]" }; //Массив масок для обработки символов ключа
        int id = 0;
        string defaultMessage = "Однажды я гулял по песку, было классно";//стандартное сообщение
        string defaultKey = "Солнце";//стандартный ключ
        public Form1()
        {
            InitializeComponent();
            openFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";//фильтры для
            saveFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";//работы с файловой системой
            comboBox1.SelectedIndex = 0;
            Console.WriteLine($"{alphabet[0].Length} + {alphabet[1].Length} + {alphabet[2].Length} + {alphabet[3].Length}"); //вывод мощности алфавитов в консоль
        }
        public char[] alphabetGenerator(string alphabetLetters = null) //Генератор алфавита в массив символов из строки
        {
            char[] alphabet = alphabetLetters.ToCharArray();
            return alphabet;
        }

        private string sentenceGenerator(string line)//Генератор предложений, приводящий строку к "человеческому" виду
        {
            string result = "";
            char[] sym = line.ToUpper().ToCharArray();
            result += sym[0];

            for (int i = 1; i< sym.Length; i++)
            {
                if(!Regex.IsMatch(sym[i-1].ToString(),@"[.!?]"))
                {

                    try //обработчик исключений на случай ошибки в начале строки
                    {
                        if ((sym[i - 1] == ' ') && Regex.IsMatch(sym[i - 2].ToString(), @"[.!?]"))
                        {
                            result += sym[i];
                        }
                        else
                        {
                            result += (sym[i].ToString()).ToLower();
                        }
                    } catch //действия в случае ошибки
                    {
                        Console.WriteLine("Ошибка, скорее всего в начале строки два пробела");
                        result += sym[i];
                    }
                } else
                {
                    result += sym[i];
                }
            }
            return result;
        }
        
        private string Encrypt(string message, string key, char[] alphabet)//Шифровальщик
        {
            message = message.ToUpper(); //перевод сообщения и ключа
            key = key.ToUpper();//в верхний регистр

            int alphabetLength = alphabet.Length;//получение длины алфавита
            string result = "";
            int key_index = 0;//индекс символа в ключе, для повторения ключа если строка больше ключа

            foreach (char element in message) //перебор каждого символа в сообщении
            {
                int c = (Array.IndexOf(alphabet, element) + Array.IndexOf(alphabet, key[key_index])) % alphabetLength;
                result += alphabet[c];
                key_index++;
                if ((key_index + 1) == key.Length)//обнуление индекса символа ключа
                    key_index = 0;
            }
            result = sentenceGenerator(result);//вызов генератора предложений 
            return result;
        }
        private string Decrypt(string message, string key, char[] alphabet)//Дешифровщик
        {
            message = message.ToUpper();//перевод сообщения и ключа
            key = key.ToUpper();//в верхний регистр

            int alphabetLength = alphabet.Length;//получение длины алфавита
            string result = "";
            int key_index = 0;

            foreach (char element in message)//перебор каждого символа в сообщении
            { 
                int p = (Array.IndexOf(alphabet, element) + alphabetLength - Array.IndexOf(alphabet, key[key_index])) % alphabetLength;
                result += alphabet[p];
                key_index++;
                if ((key_index + 1) == key.Length)//обнуление индекса символа ключа
                    key_index = 0;
            }
            result = sentenceGenerator(result); //вызов генератора предложений 
            return result;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            button4.Enabled = true;
            if ((textBox1.Text == "") || (textBox2.Text == "")) {
                textBox1.Text = defaultMessage;
                textBox2.Text = defaultKey;
                label4.Visible = true;
            } else
            {
                label4.Visible = false;
            }
            if (radioButton1.Checked == true) {
                textBox3.Text = Encrypt(textBox1.Text, textBox2.Text, alphabetGenerator(alphabet[id])).ToString() ;
            }
            if (radioButton2.Checked == true)
            {
                textBox3.Text = Decrypt(textBox1.Text, textBox2.Text, alphabetGenerator(alphabet[id])).ToString();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            id = comboBox1.SelectedIndex;
            textBox2.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if ((textBox2.Text != "") && Regex.IsMatch(textBox2.Text, @mask[id]) && !Regex.IsMatch(textBox2.Text,@"\d")) //проверка ключа на валидность
            {
                button1.Enabled = true;
                label5.Visible = false;
            } else
            {
                button1.Enabled = false;
                label5.Visible = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = new StreamReader(openFileDialog1.FileName);
                textBox1.Text = sentenceGenerator(File.ReadLines(openFileDialog1.FileName).First());
                sr.Close();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog1.FileName;
                StreamWriter sw = new StreamWriter(filename, false, Encoding.UTF8);
                sw.WriteLine(textBox3.Text);
                sw.Close();
            }
        }
    }
}
