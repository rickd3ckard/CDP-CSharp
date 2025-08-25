/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.Text.Json;

namespace CDP.Utils
{
    public class BoxModel
    {
        public BoxModel(JsonDocument Model)
        {
            JsonElement modelElement; 
            if (!Model.RootElement.TryGetProperty("model", out modelElement)) {throw new InvalidCastException();}

            this.Content = DoubleArrayToPointArray(ExtractDoubleArray("content", modelElement));
            this.Padding = DoubleArrayToPointArray(ExtractDoubleArray("padding", modelElement));
            this.Border = DoubleArrayToPointArray(ExtractDoubleArray("border", modelElement));
            this.Margin = DoubleArrayToPointArray(ExtractDoubleArray("content", modelElement));

            this.Width = modelElement.GetProperty("width").GetDouble();
            this.Height = modelElement.GetProperty("height").GetDouble();
            this.Center = ComputeCente();
        }

        // An array of quad vertices, x immediately followed by y for each point, points clock-wise.
        public Point[] Content { get; }
        public Point[] Padding { get; }
        public Point[] Border { get; }
        public Point[] Margin { get; }
        public double Width { get; }
        public double Height { get; }
        public Point Center { get; }

        private double[] ExtractDoubleArray(string TargetProperty, JsonElement ModelElement)
        {
            double[]? doubleArray = JsonSerializer.Deserialize<double[]>(ModelElement.GetProperty(TargetProperty).GetRawText());
            if (doubleArray == null) { throw new InvalidCastException(); }
            return doubleArray;
        }

        private Point[] DoubleArrayToPointArray(double[] DoubleArray)
        {
            if (DoubleArray.Length != 8) { throw new InvalidOperationException(); }
           
            List<Point> pointList = new List<Point>();
            for (byte index = 0; index <= 7; index += 2)
            {
                double[] points = DoubleArray.Skip(index).Take(2).ToArray();
                Point newPoint = new Point(points[0], points[1]);
                pointList.Add(newPoint);
            }

            return pointList.ToArray();
        }

        private Point ComputeCente()
        {
            double X = this.Content[0].X + ((this.Content[1].X - this.Content[0].X) / 2);
            double Y = this.Content[1].Y + ((this.Content[2].Y - this.Content[1].Y) / 2);
            return new Point(X, Y);
        }

        public override string ToString()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            return JsonSerializer.Serialize(this, options);
        }
    }

    public class Point
    {
        public Point(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public double X { get; }
        public double Y { get; }

        public override string ToString()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            return JsonSerializer.Serialize(this, options);
        }
    }
}