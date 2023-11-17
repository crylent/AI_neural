using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AI_neural.UI;

public class BitCanvas: Canvas
{
    public BitImage8 Image { get; private set; } = new();

    private const int Size = BitImage8.Size;
    private const int CellSize = 50;

    private readonly Rectangle[][] _cells;

    public BitCanvas()
    {
        Background = Brushes.White;
        _cells = new Rectangle[Size][];
        for (var y = 0; y < Size; y++)
        {
            _cells[y] = new Rectangle[Size];
            for (var x = 0; x < Size; x++)
            {
                var rect = new Rectangle
                {
                    Width = CellSize,
                    Height = CellSize,
                    Stroke = Brushes.LightGray
                };
                Children.Add(rect);
                SetLeft(rect, x * CellSize);
                SetTop(rect, y * CellSize);
                _cells[y][x] = rect;
            }
        }
        StartDrawing();
    }

    private bool _leftButtonIsDown;
    private bool _isErasing;

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        OnPointTouch(e.GetPosition(this));
        _leftButtonIsDown = true;
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);
        _leftButtonIsDown = false;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (!_leftButtonIsDown) return;
        OnPointTouch(e.GetPosition(this));
    }

    private void OnPointTouch(Point position)
    {
        var x = (int) (position.X / CellSize);
        var y = (int) (position.Y / CellSize);
        if (_isErasing)
        {
            Image.ClearPoint(x, y);
            DrawPoint(x, y, Brushes.White);
        }
        else
        {
            Image.DrawPoint(x, y);
            DrawPoint(x, y, Brushes.Black);
        }
    }

    private void DrawPoint(int x, int y, Brush color)
    {
        _cells[y][x].Fill = color;
    }

    public void StartDrawing()
    {
        Cursor = Cursors.Pen;
        _isErasing = false;
    }

    public void StartErasing()
    {
        Cursor = Cursors.Cross;
        _isErasing = true;
    }

    public void Clear()
    {
        Image = new BitImage8();
        for (var y = 0; y < Size; y++)
        {
            for (var x = 0; x < Size; x++)
            {
                _cells[y][x].Fill = Brushes.White;
            }
        }
    }

    public void LoadImage(BitImage8 image)
    {
        Image = image;
        for (var y = 0; y < Size; y++)
        {
            for (var x = 0; x < Size; x++)
            {
                _cells[y][x].Fill = image.GetPoint(x, y) > 0 ? Brushes.Black : Brushes.White;
            }
        }
    }
}