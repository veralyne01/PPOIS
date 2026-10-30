using System;

namespace PostMachine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int input;
            Slider slider = new Slider();
            Console.WriteLine("----------------------\nPOST MACHINE IMITATION\n----------------------\n");
            do
            {
                input = 0;
                Console.WriteLine($"CURRENT POSITION: {slider.Position}\n" +
                    "Choose the command:\n1. New label \n2. Remove label \n3. Move left \n4. Move right \n5. New tape \n6. Check cell \n7. Exit");
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
                    default:
                        break;
                }
                if (!(Console.IsOutputRedirected)) Console.Clear();
            } while (input != 7);
            return;
        }
    }
}