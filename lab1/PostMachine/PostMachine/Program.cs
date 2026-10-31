using System;
using System.Collections.Generic;

namespace PostMachine
{
    /// <summary>
    /// Точка входа консольного имитатора машины Поста.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Запускает главное меню: принимает команды пользователя
        /// (метки, сдвиги каретки, новая лента, проверка клетки, ввод программы)
        /// и выполняет их, пока не выбран пункт выхода (8).
        /// </summary>
        /// <param name="args">Аргументы командной строки (не используются).</param>
        public static void Main(string[] args)
        {
            int input;
            Slider slider = new Slider();
            Console.WriteLine("----------------------\nPOST MACHINE IMITATION\n----------------------\n");
            do
            {
                input = 0;
                Console.WriteLine($"CURRENT POSITION: {slider.Position}\n" +
                    "Choose the command:\n1. New label \n2. Remove label \n3. Move left \n4. Move right \n5. New tape \n6. Check cell \n7. Enter program \n8. Exit");
                try
                {
                    input = InputValidator.ValidateInt();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
                switch (input)
                {
                    case 1:
                        slider.MakeLabel();
                        break;
                    case 2:
                        slider.RemoveLabel();
                        break;
                    case 3:
                        try
                        {
                            int pos = InputValidator.ValidateInt();
                            slider.MoveLeft(pos);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Console.ReadLine();
                        }
                        break;
                    case 4:
                        try
                        {
                            int pos = InputValidator.ValidateInt();
                            slider.MoveRight(pos);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Console.ReadLine();
                        }
                        break;
                    case 5:
                        slider.Reset();
                        break;
                    case 6:
                        if (slider.CheckCell())
                        {
                            Console.WriteLine("Current cell is already labelled\n");
                            Console.ReadLine();
                        }
                        else
                        {
                            Console.WriteLine("Current cell is not labelled\n");
                            Console.ReadLine();
                        }
                        break;
                    case 7:
                        try
                        {
                            Console.WriteLine("Enter program lines (empty line to finish):");
                            var lines = new List<string>();
                            string? line;
                            while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
                                lines.Add(line);

                            var program = ProgramParser.Parse(lines);
                            ConsoleVisualizer.Run(slider, program);
                            Console.WriteLine("Program finished\n");
                            Console.WriteLine("Program finished\n");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        Console.ReadLine();
                        break;
                    default:
                        break;
                }
                if (!(Console.IsOutputRedirected)) Console.Clear();
            } while (input != 8);
            return;
        }
    }
}