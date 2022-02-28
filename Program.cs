using System;
using System.Collections.Generic;
using System.Linq;

namespace hepsiburada_turkerurganci_casestudy
{
    class Program
    {
        static void Main(string[] args)
        {
            var counter = 0;
            var roverManager = new RoverManager();
            var rover = new Rover();

            while (true)
            {
                string letter = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(letter) && counter == 0)
                {
                    var letterChars = letter.Split(' ').Select(t => Convert.ToInt32(t)).ToArray();
                    var res = roverManager.SetUpperRightCoordinates(letterChars[0], letterChars[1]);

                    if (!res.Success)
                    {
                        Console.WriteLine(res.Message);
                        Console.ReadLine();
                    }
                }
                else if (!string.IsNullOrWhiteSpace(letter) && counter % 2 == 1)
                {
                    var letterChars = letter.Split(' ');
                    var res = rover.SetPosition(Convert.ToInt32(letterChars[0]), Convert.ToInt32(letterChars[1]), letterChars[2].First());

                    if (!res.Success)
                    {
                        Console.WriteLine(res.Message);
                        Console.ReadLine();
                    }
                }
                else if (!string.IsNullOrWhiteSpace(letter) && counter % 2 == 0)
                {
                    rover.SetLetters(letter.Trim());
                    roverManager.AddRover(rover);
                    rover = new Rover();
                }
                else if (string.IsNullOrWhiteSpace(letter))
                {
                    var res = roverManager.ApplyRoversCommands();
                    if (!res.Success)
                    {
                        Console.WriteLine(res.Message);
                        Console.ReadLine();
                    }

                    Console.WriteLine(string.Join(Environment.NewLine, roverManager.GetRovers()?.Select(t => t.GetPosition())));
                    Console.ReadLine();
                }
                else
                {
                    throw new NotImplementedException();
                }

                counter++;
            }
        }
    }

    public class RoverManager
    {
        private ICollection<Rover> Rovers;
        private int _maxX = 0;
        private int _maxY = 0;

        public RoverManager()
        {
            Rovers = new List<Rover>();
        }

        public MethodResult SetUpperRightCoordinates(int maxX, int maxY)
        {
            if (maxX <= 0 || maxY <= 0)
            {
                return new MethodResult { Success = false, Message = "Values must be grater than zero(0)" };
            }

            _maxX = maxX;
            _maxY = maxY;

            return new MethodResult { Success = true };
        }

        public void AddRover(Rover rover) => Rovers.Add(rover);

        public ICollection<Rover> GetRovers() { return Rovers; }

        public MethodResult ApplyRoversCommands()
        {
            try
            {
                foreach (var rover in Rovers)
                {
                    foreach (var command in rover.Letters)
                    {
                        switch (command)
                        {
                            case 'L':
                                rover.TurnLeft();
                                break;
                            case 'R':
                                rover.TurnRight();
                                break;
                            case 'M':
                                rover.Move();
                                var res = CheckPosition(rover);
                                if (!res.Success)
                                {
                                    return new MethodResult { Success = false, Message = res.Message };
                                }
                                break;
                            default:
                                return new MethodResult { Success = false, Message = "Unexpected command" };
                        }
                    }
                }

                return new MethodResult { Success = true };
            }
            catch (Exception ex)
            {
                return new MethodResult { Success = false, Message = ex.Message };
            }
        }

        public MethodResult CheckPosition(Rover rover)
        {
            if (rover.GetX() > _maxX || rover.GetX() < 0 || rover.GetY() > _maxY || rover.GetY() < 0)
            {
                return new MethodResult { Success = false, Message = "Rover was out of region" };
            }

            return new MethodResult { Success = true };
        }
    }

    public class Rover
    {
        private int x = 0;
        private int y = 0;
        private ICompass compassPoint;
        public string Letters { get; private set; }

        public int GetX() { return x; }

        public int GetY() { return y; }

        public MethodResult SetPosition(int initialX, int initialY, char initialCompassPoint)
        {
            try
            {
                x = initialX;
                y = initialY;
                compassPoint = CompassManager.GetCompassByPointValue(initialCompassPoint);

                if (compassPoint == null)
                {
                    return new MethodResult { Success = false, Message = "No compass for given value" };
                }

                return new MethodResult { Success = true };
            }
            catch (Exception ex)
            {
                return new MethodResult { Success = false, Message = ex.Message };
            }
        }

        public void SetLetters(string s) { Letters = s; }

        public void TurnLeft() { compassPoint = compassPoint.LeftPoint; }

        public void TurnRight() { compassPoint = compassPoint.RightPoint; }

        public void Move() { compassPoint.Move(ref x, ref y); }

        public string GetPosition()
        {
            return string.Concat(x, " ", y, " ", compassPoint.PointValue);
        }
    }

    public interface ICompass
    {
        char PointValue { get; }
        ICompass LeftPoint { get; }
        ICompass RightPoint { get; }

        void Move(ref int x, ref int y);
    }

    public class NorthCompass : ICompass
    {
        public char PointValue { get { return 'N'; } }
        public ICompass LeftPoint { get { return new WestCompass(); } }
        public ICompass RightPoint { get { return new EastCompass(); } }

        public void Move(ref int x, ref int y) => y++;
    }

    public class EastCompass : ICompass
    {
        public char PointValue { get { return 'E'; } }
        public ICompass LeftPoint { get { return new NorthCompass(); } }
        public ICompass RightPoint { get { return new SouthCompass(); } }

        public void Move(ref int x, ref int y) => x++;
    }

    public class SouthCompass : ICompass
    {
        public char PointValue { get { return 'S'; } }
        public ICompass LeftPoint { get { return new EastCompass(); } }
        public ICompass RightPoint { get { return new WestCompass(); } }

        public void Move(ref int x, ref int y) => y--;
    }

    public class WestCompass : ICompass
    {
        public char PointValue { get { return 'W'; } }
        public ICompass LeftPoint { get { return new SouthCompass(); } }
        public ICompass RightPoint { get { return new NorthCompass(); } }

        public void Move(ref int x, ref int y) => x--;
    }

    public static class CompassManager
    {
        public static readonly List<ICompass> CompassList = new List<ICompass> { new NorthCompass(), new EastCompass(), new SouthCompass(), new WestCompass() };

        public static ICompass GetCompassByPointValue(char point) => CompassList.FirstOrDefault(t => t.PointValue == point);
    }

    public class MethodResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
