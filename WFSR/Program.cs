using System;
using System.Drawing;
using System.Windows.Forms;

public class CSSR : Form
{
    private Timer timer; // This is our tick system

    // Some basic constants
    private const int Width = 640;
    private const int Height = 480;
    private const int CubeSize = 20; // Debug
    private const int MoveAmount = 10;
    private const float RotationAngle = 0.1f;

    private readonly Bitmap bitmap;
    private readonly Graphics graphics;
    private Point3D cameraPosition;
    private float cubeRotationZ;
    private float cubeRotationX;

    public CSSR()
    {
        bitmap = new Bitmap(Width, Height);
        graphics = Graphics.FromImage(bitmap);
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
        ClientSize = new Size(Width, Height);
        KeyDown += CSSR_KeyDown;

        // Set initial camera position
        cameraPosition = new Point3D(0, 0, 500);

        // Initialize the timer
        timer = new Timer();
        timer.Interval = 50; // Update interval in milliseconds
        timer.Tick += Timer_Tick;
        timer.Start();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        // Update cube rotation
        cubeRotationZ += RotationAngle;
        cubeRotationX += RotationAngle;

        // Invalidate the form to trigger repaint
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        RenderCube();
        e.Graphics.DrawImage(bitmap, 0, 0);
    }

    private void RenderCube()
    {
        // Define the vertices of the cube
        var vertices = new[]
        {
            new Point3D(-CubeSize / 2, -CubeSize / 2, -CubeSize / 2),
            new Point3D(CubeSize / 2, -CubeSize / 2, -CubeSize / 2),
            new Point3D(CubeSize / 2, CubeSize / 2, -CubeSize / 2),
            new Point3D(-CubeSize / 2, CubeSize / 2, -CubeSize / 2),
            new Point3D(-CubeSize / 2, -CubeSize / 2, CubeSize / 2),
            new Point3D(CubeSize / 2, -CubeSize / 2, CubeSize / 2),
            new Point3D(CubeSize / 2, CubeSize / 2, CubeSize / 2),
            new Point3D(-CubeSize / 2, CubeSize / 2, CubeSize / 2)
        };

        // Define the edges of the cube
        var edges = new[]
        {
            (0, 1), (1, 2), (2, 3), (3, 0),
            (4, 5), (5, 6), (6, 7), (7, 4),
            (0, 4), (1, 5), (2, 6), (3, 7)
        };

        // Clear the screen
        graphics.Clear(Color.Black);

        // Apply camera position and cube rotations to vertices
        var transformedVertices = new Point3D[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            var rotatedVertex = RotateVertex(vertices[i]);
            transformedVertices[i] = rotatedVertex + cameraPosition;
        }

        // Draw text at the top-left corner of the form
        string text = "Hello, CSSR!";
        Font font = new Font("Consolas", 12, FontStyle.Bold);
        Brush brush = Brushes.White;
        PointF textPosition = new PointF(10, 10);
        graphics.DrawString(text, font, brush, textPosition);

        // Render the edges of the cube
        foreach (var edge in edges)
        {
            var p1 = transformedVertices[edge.Item1];
            var p2 = transformedVertices[edge.Item2];
            DrawLine(p1, p2);
        }
    }

    private Point3D RotateVertex(Point3D vertex)
    {
        // Rotate the vertex around the Z-axis
        var cosThetaZ = Math.Cos(cubeRotationZ);
        var sinThetaZ = Math.Sin(cubeRotationZ);
        var x = vertex.X * cosThetaZ - vertex.Y * sinThetaZ;
        var y = vertex.X * sinThetaZ + vertex.Y * cosThetaZ;
        var z = vertex.Z;

        // Rotate the vertex around the X-axis
        var cosThetaX = Math.Cos(cubeRotationX);
        var sinThetaX = Math.Sin(cubeRotationX);
        var rotatedY = y * cosThetaX - z * sinThetaX;
        var rotatedZ = y * sinThetaX + z * cosThetaX;

        return new Point3D((int)x, (int)rotatedY, (int)rotatedZ);
    }

    private void DrawLine(Point3D p1, Point3D p2)
    {
        var startPoint = new PointF(p1.X + Width / 2, p1.Y + Height / 2);
        var endPoint = new PointF(p2.X + Width / 2, p2.Y + Height / 2);
        graphics.DrawLine(Pens.White, startPoint, endPoint);
    }

    private void CSSR_KeyDown(object sender, KeyEventArgs e)
    {
        // Move the cube using arrow keys
        switch (e.KeyCode)
        {
            case Keys.Up:
                cameraPosition.Y -= MoveAmount;
                break;
            case Keys.Down:
                cameraPosition.Y += MoveAmount;
                break;
            case Keys.Left:
                cameraPosition.X -= MoveAmount;
                break;
            case Keys.Right:
                cameraPosition.X += MoveAmount;
                break;
        }

        Invalidate();
    }

    private class Point3D
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Point3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Point3D operator +(Point3D p1, Point3D p2)
        {
            return new Point3D(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
        }
    }

    public static void Main()
    {
        Application.Run(new CSSR());
    }
}
