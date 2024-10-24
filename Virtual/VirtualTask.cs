using System;
using System.Collections.Generic;
using System.Linq;

namespace Inheritance.Geometry.Virtual {
    // *************************************************************************************************
    public abstract class Body { // абстрактный базовый класс для всех фигур
        public Vector3 Position { get; } // позиция центра фигуры

        protected Body(Vector3 position) { // конструктор принимает позицию
            Position = position;
        }

        public abstract bool ContainsPoint(Vector3 point); // абстрактный метод для проверки принадлежности точки фигуре

        public abstract RectangularCuboid GetBoundingBox(); // абстрактный метод для получения ограничительной рамки
    }

    // *************************************************************************************************
    public class Ball : Body { // класс для шара
        public double Radius { get; } // радиус шара

        public Ball(Vector3 position, double radius) : base(position) { // конструктор класса
            Radius = radius;
        }

        public override bool ContainsPoint(Vector3 point) { // переопределение метода ContainsPoint
            var vector = point - Position;
            var length2 = vector.GetLength2();
            return length2 <= Radius * Radius;
        }

        public override RectangularCuboid GetBoundingBox() { // переопределение метода GetBoundingBox 
            var size = 2 * Radius;
            return new RectangularCuboid(Position, size, size, size);
        }
    }

    // *************************************************************************************************
    public class RectangularCuboid : Body // класс для прямоугольного параллелепипеда
    {
        public double SizeX { get; } // длина по X
        public double SizeY { get; } // длина по Y
        public double SizeZ { get; } // длина по Z

        public RectangularCuboid(Vector3 position, double sizeX, double sizeY, double sizeZ) : base(position) { // конструктор класса
            SizeX = sizeX;
            SizeY = sizeY;
            SizeZ = sizeZ;
        }

        public override bool ContainsPoint(Vector3 point) { // переопределение метода ContainsPoint для прямоугольного параллелепипеда
            var minPoint = new Vector3(Position.X - SizeX / 2, Position.Y - SizeY / 2, Position.Z - SizeZ / 2); // координаты минимальной точки параллелепипеда
            var maxPoint = new Vector3(Position.X + SizeX / 2, Position.Y + SizeY / 2, Position.Z + SizeZ / 2); // координаты максимальной точки параллелепипеда

            return point >= minPoint && point <= maxPoint; // проверяем, принадлежит точка параллелепипеду?
        }

        public override RectangularCuboid GetBoundingBox() { // переопределение метода GetBoundingBox 
            return this; // ограничительная рамка - сам параллелепипед
        }
    }

    // *************************************************************************************************
    public class Cylinder : Body { // класс для цилиндра
        public double SizeZ { get; } // высота цилиндра
        public double Radius { get; } // радиус цилиндра

        public Cylinder(Vector3 position, double sizeZ, double radius) : base(position) { // конструктор класса
            SizeZ = sizeZ;
            Radius = radius;
        }

        public override bool ContainsPoint(Vector3 point) { // переопределение метода ContainsPoint для цилиндра
            var vectorX = point.X - Position.X;
            var vectorY = point.Y - Position.Y;
            var length2 = vectorX * vectorX + vectorY * vectorY;
            var minZ = Position.Z - SizeZ / 2;
            var maxZ = minZ + SizeZ;

            return length2 <= Radius * Radius && point.Z >= minZ && point.Z <= maxZ; // проверяем, принадлежит точка цилиндру
        }

        public override RectangularCuboid GetBoundingBox() { // переопределение метода GetBoundingBox для цилиндра
            var size = 2 * Radius;
            return new RectangularCuboid(Position, size, size, SizeZ); // ограничительная рамка для цилиндра 
        }
    }

    // *************************************************************************************************
    public class CompoundBody : Body { // класс для составной фигуры
        public IReadOnlyList<Body> Parts { get; } // список составляющих фигур

        public CompoundBody(IReadOnlyList<Body> parts) : base(parts[0].Position) {  // конструктор класса
            Parts = parts; // позиция составной фигуры равна позиции первой фигуры в списке
        }

        public override bool ContainsPoint(Vector3 point) { // переопределение метода ContainsPoint для составной фигуры
            return Parts.Any(body => body.ContainsPoint(point)); // точка принадлежит составной фигуре? 
        }

        public override RectangularCuboid GetBoundingBox() { // переопределение метода GetBoundingBox для составной фигуры
            var minX = double.MaxValue;
            var minY = double.MaxValue;
            var minZ = double.MaxValue;
            var maxX = double.MinValue;
            var maxY = double.MinValue;
            var maxZ = double.MinValue;

            foreach (var part in Parts) { 
                var box = part.GetBoundingBox();
                var min = box.Position - new Vector3(box.SizeX / 2, box.SizeY / 2, box.SizeZ / 2); // минимальная точка ограничительной рамки 
                var max = box.Position + new Vector3(box.SizeX / 2, box.SizeY / 2, box.SizeZ / 2); // максимальная точка ограничительной рамки

                minX = Math.Min(minX, min.X);
                minY = Math.Min(minY, min.Y);
                minZ = Math.Min(minZ, min.Z);
                maxX = Math.Max(maxX, max.X);
                maxY = Math.Max(maxY, max.Y);
                maxZ = Math.Max(maxZ, max.Z);
            }

            return new RectangularCuboid(new Vector3((maxX + minX) / 2, (maxY + minY) / 2, (maxZ + minZ) / 2), maxX - minX, maxY - minY, maxZ - minZ); // ограничительная рамка
        }
    }
    // *************************************************************************************************
}
