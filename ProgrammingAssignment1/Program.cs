using System;

namespace ProgrammingAssignment1
{
    class Program
    {
        static void Main(string[] args)
        {
            /// Welcome message
            Console.WriteLine("Welcome! Let's do programming with C# ! ");
            Console.WriteLine("This program will calculate the distance of (x_1, y_1) and (x_2, y_2)");
            Console.WriteLine("So we need you to input 4 numbers.");

            /// Read 4 numbers
            Console.Write("Point 1 X: ");
            float point1X = float.Parse(Console.ReadLine());
            Console.Write("Point 1 Y: ");
            float point1Y = float.Parse(Console.ReadLine());
            Console.Write("Point 2 X: ");
            float point2X = float.Parse(Console.ReadLine());
            Console.Write("Point 2 Y: ");
            float point2Y = float.Parse(Console.ReadLine());

            /// Do some calculations
            float deltaX = point2X - point1X;
            float deltaY = point2Y - point1Y;
            float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            float angleRad = (float)Math.Atan2(deltaY, deltaX);
            float angleDeg = (float)(Math.Atan2(deltaY, deltaX) * 180 / Math.PI);

            /// And output !
            //Console.WriteLine("The distance between ({0:F3}, {1:F3}) and ({2:F3}, {3:F3}) is {4:F3}", point1X, point1Y, point2X, point2Y, distance);
            //Console.WriteLine("The angle between ({0:F3}, {1:F3}) and ({2:F3}, {3:F3}) is {4:F3} degrees", point1X, point1Y, point2X, point2Y, angleDeg);
            /// A oh... We have to use expected output format
            Console.WriteLine("Distance between points: {4:F3}", point1X, point1Y, point2X, point2Y, distance);
            Console.WriteLine("Angle between points: {4:F3} degrees", point1X, point1Y, point2X, point2Y, angleDeg);
            
            // Pause the output and wait for exit.
            Console.WriteLine("Finished. Press any key to exit...");
            Console.ReadKey();

        }
    }
}
