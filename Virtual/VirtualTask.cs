using System;
using System.Collections.Generic;
using System.Linq;

namespace Inheritance.Geometry.Virtual {
    // *************************************************************************************************
    public abstract class Body { // ����������� ������� ����� ��� ���� �����
        public Vector3 Position { get; } // ������� ������ ������

        protected Body(Vector3 position) { // ����������� ��������� �������
            Position = position;
        }

        public abstract bool ContainsPoint(Vector3 point); // ����������� ����� ��� �������� �������������� ����� ������

        public abstract RectangularCuboid GetBoundingBox(); // ����������� ����� ��� ��������� ��������������� �����
    }

    // *************************************************************************************************
    public class Ball : Body { // ����� ��� ����
        public double Radius { get; } // ������ ����

        public Ball(Vector3 position, double radius) : base(position) { // ����������� ������
            Radius = radius;
        }

        public override bool ContainsPoint(Vector3 point) { // ��������������� ������ ContainsPoint
            var vector = point - Position;
            var length2 = vector.GetLength2();
            return length2 <= Radius * Radius;
        }

        public override RectangularCuboid GetBoundingBox() { // ��������������� ������ GetBoundingBox 
            var size = 2 * Radius;
            return new RectangularCuboid(Position, size, size, size);
        }
    }

    // *************************************************************************************************
    public class RectangularCuboid : Body // ����� ��� �������������� ���������������
    {
        public double SizeX { get; } // ����� �� X
        public double SizeY { get; } // ����� �� Y
        public double SizeZ { get; } // ����� �� Z

        public RectangularCuboid(Vector3 position, double sizeX, double sizeY, double sizeZ) : base(position) { // ����������� ������
            SizeX = sizeX;
            SizeY = sizeY;
            SizeZ = sizeZ;
        }

        public override bool ContainsPoint(Vector3 point) { // ��������������� ������ ContainsPoint ��� �������������� ���������������
            var minPoint = new Vector3(Position.X - SizeX / 2, Position.Y - SizeY / 2, Position.Z - SizeZ / 2); // ���������� ����������� ����� ���������������
            var maxPoint = new Vector3(Position.X + SizeX / 2, Position.Y + SizeY / 2, Position.Z + SizeZ / 2); // ���������� ������������ ����� ���������������

            return point >= minPoint && point <= maxPoint; // ���������, ����������� ����� ���������������?
        }

        public override RectangularCuboid GetBoundingBox() { // ��������������� ������ GetBoundingBox 
            return this; // ��������������� ����� - ��� ��������������
        }
    }

    // *************************************************************************************************
    public class Cylinder : Body { // ����� ��� ��������
        public double SizeZ { get; } // ������ ��������
        public double Radius { get; } // ������ ��������

        public Cylinder(Vector3 position, double sizeZ, double radius) : base(position) { // ����������� ������
            SizeZ = sizeZ;
            Radius = radius;
        }

        public override bool ContainsPoint(Vector3 point) { // ��������������� ������ ContainsPoint ��� ��������
            var vectorX = point.X - Position.X;
            var vectorY = point.Y - Position.Y;
            var length2 = vectorX * vectorX + vectorY * vectorY;
            var minZ = Position.Z - SizeZ / 2;
            var maxZ = minZ + SizeZ;

            return length2 <= Radius * Radius && point.Z >= minZ && point.Z <= maxZ; // ���������, ����������� ����� ��������
        }

        public override RectangularCuboid GetBoundingBox() { // ��������������� ������ GetBoundingBox ��� ��������
            var size = 2 * Radius;
            return new RectangularCuboid(Position, size, size, SizeZ); // ��������������� ����� ��� �������� 
        }
    }

    // *************************************************************************************************
    public class CompoundBody : Body { // ����� ��� ��������� ������
        public IReadOnlyList<Body> Parts { get; } // ������ ������������ �����

        public CompoundBody(IReadOnlyList<Body> parts) : base(parts[0].Position) {  // ����������� ������
            Parts = parts; // ������� ��������� ������ ����� ������� ������ ������ � ������
        }

        public override bool ContainsPoint(Vector3 point) { // ��������������� ������ ContainsPoint ��� ��������� ������
            return Parts.Any(body => body.ContainsPoint(point)); // ����� ����������� ��������� ������? 
        }

        public override RectangularCuboid GetBoundingBox() { // ��������������� ������ GetBoundingBox ��� ��������� ������
            var minX = double.MaxValue;
            var minY = double.MaxValue;
            var minZ = double.MaxValue;
            var maxX = double.MinValue;
            var maxY = double.MinValue;
            var maxZ = double.MinValue;

            foreach (var part in Parts) { 
                var box = part.GetBoundingBox();
                var min = box.Position - new Vector3(box.SizeX / 2, box.SizeY / 2, box.SizeZ / 2); // ����������� ����� ��������������� ����� 
                var max = box.Position + new Vector3(box.SizeX / 2, box.SizeY / 2, box.SizeZ / 2); // ������������ ����� ��������������� �����

                minX = Math.Min(minX, min.X);
                minY = Math.Min(minY, min.Y);
                minZ = Math.Min(minZ, min.Z);
                maxX = Math.Max(maxX, max.X);
                maxY = Math.Max(maxY, max.Y);
                maxZ = Math.Max(maxZ, max.Z);
            }

            return new RectangularCuboid(new Vector3((maxX + minX) / 2, (maxY + minY) / 2, (maxZ + minZ) / 2), maxX - minX, maxY - minY, maxZ - minZ); // ��������������� �����
        }
    }
    // *************************************************************************************************
}
