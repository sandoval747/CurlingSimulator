using CurlingSimulator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CurlingSimulator.Controllers
{
    public class GraphicsController : Controller
    {
        private readonly ILogger<GraphicsController> _logger;

        public GraphicsController(ILogger<GraphicsController> logger)
        {
            _logger = logger;
        }

        public IActionResult GetCurlingChart(string disks)
        {
            FileContentResult image;
            var diskArray = JsonConvert.DeserializeObject<Disk[]>(disks);

            int maxX = 0;
            int maxY = 0;

            //Find maximum X and Y coordinates on chart to draw disks within bounds
            foreach (var disk in diskArray)
            {
                if ((disk.CenterPoint.X + disk.Radius) > maxX) maxX = (int)Math.Ceiling(disk.CenterPoint.X + disk.Radius);
                if ((disk.CenterPoint.Y + disk.Radius) > maxY) maxY = (int)Math.Ceiling(disk.CenterPoint.Y + disk.Radius);
            }            
            
            //Adjust max X and Y to make chart aesthetically pleasing without too much/too little whitespace
            maxX = (int)(Math.Round(maxX / 5d, 0) * 5 + 1) > maxX ? (int)(Math.Round(maxX / 5d, 0) * 5 + 1) : maxX + 1;
            maxY = (int)(Math.Round(maxY / 5d, 0) * 5 + 1) > maxY ? (int)(Math.Round(maxY / 5d, 0) * 5 + 1) : maxY + 1;

            //Set scale of the chart relative to X/Y to create images of roughly the same size no matter the data
            int scale = Math.Max(1500 / maxX, 1500 / maxY);
            
            var margin = scale / 10;
            var axisWidth = scale;

            var chartArea = new RectangleF(new PointF(margin, margin), new SizeF(axisWidth + maxX * scale, axisWidth + maxY * scale));
            var dataArea = new RectangleF(new PointF(margin + axisWidth, margin), new SizeF(maxX * scale, maxY * scale));

            try
            {
                using (var stream = new MemoryStream())
                {
                    using (var bitmap = new Bitmap(margin * 2 + (int)chartArea.Width, margin * 2 + (int)chartArea.Height))
                    {
                        using (var graphics = Graphics.FromImage(bitmap))
                        {
                            using (var pen = new Pen(Color.Black, scale / 25))
                            {
                                DrawAxes(maxX, maxY, axisWidth, chartArea, graphics, pen, scale);

                                foreach (var disk in diskArray)
                                {
                                    DrawDisk(disk, dataArea, graphics, pen, scale);
                                }
                            }
                        }

                        bitmap.Save(stream, ImageFormat.Png);

                        image = File(stream.ToArray(), "image/png");
                    }
                }

                return image;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating chart image");
                return null;
            }
        }

        private void DrawAxes(int maxX, int maxY, float axisWidth, RectangleF drawArea, Graphics graphics, Pen pen, int scale)
        {
            //Draw Axes
            graphics.DrawLines(pen, new PointF[] {
                new PointF(drawArea.Left + axisWidth, drawArea.Top),
                new PointF(drawArea.Left + axisWidth, drawArea.Bottom - axisWidth),
                new PointF(drawArea.Right, drawArea.Bottom - axisWidth)
            });
            
            using (var font = new Font("Arial", 0.3f * scale))
            {
                var textFormat = new StringFormat();
                textFormat.Alignment = StringAlignment.Center;
                textFormat.LineAlignment = StringAlignment.Center;

                //X-Axis Labels
                var labelAreaX = new RectangleF(drawArea.Left + axisWidth, drawArea.Bottom - axisWidth, drawArea.Width - axisWidth, axisWidth);

                var xAxisMinInterval = maxX > 11 ? 5 : 2;
                var xAxisMaxNumLabels = 5;
                int xInterval = (int)Math.Round((double)maxX / (xAxisMinInterval * xAxisMaxNumLabels), 0) * xAxisMinInterval;
                var xLabels = new int[5]
                {
                    0, xInterval, xInterval * 2, xInterval * 3, xInterval * 4
                };

                foreach (var label in xLabels)
                {
                    if (label <= maxX)
                    {
                        var tickStart = new PointF(labelAreaX.Left + label * scale, labelAreaX.Top);
                        var tickEnd = new PointF(labelAreaX.Left + label * scale, labelAreaX.Top + 0.2f * axisWidth);
                        var textArea = new RectangleF(tickEnd.X - font.Height * 2, tickEnd.Y + 0.4f * axisWidth - font.Height / 2, font.Height * 4, font.Height);
                        
                        graphics.DrawLine(pen, tickStart, tickEnd);
                        graphics.DrawString(label.ToString(), font, pen.Brush, textArea, textFormat);
                    }
                }

                //Y-Axis Labels
                var labelAreaY = new RectangleF(drawArea.X, drawArea.Top, axisWidth, drawArea.Height - axisWidth);

                var yAxisMinInterval = maxY > 11 ? 5 : 2;
                var yAxisMaxNumLabels = 5;
                int yInterval = (int)Math.Round((double)maxY / (yAxisMinInterval * yAxisMaxNumLabels), 0) * yAxisMinInterval;
                var yLabels = new int[5]
                {
                    0, yInterval, yInterval * 2, yInterval * 3, yInterval * 4
                };

                foreach (var label in yLabels)
                {
                    if (label <= maxY)
                    {
                        var tickStart = new PointF(labelAreaY.Right, labelAreaY.Bottom - label * scale);
                        var tickEnd = new PointF(labelAreaY.Right - 0.2f * axisWidth, labelAreaY.Bottom - label * scale);
                        var textArea = new RectangleF(tickEnd.X - 0.4f * axisWidth - font.Height * 2, tickEnd.Y - font.Height / 2, font.Height * 4, font.Height);

                        graphics.DrawLine(pen, tickStart, tickEnd);
                        graphics.DrawString(label.ToString(), font, pen.Brush, textArea, textFormat);
                    }
                }
            }                   
        }

        private void DrawDisk(Disk disk, RectangleF drawArea, Graphics graphics, Pen pen, int scale)
        {
            var x = drawArea.Left + (float)(disk.CenterPoint.X - disk.Radius) * scale;
            var y = drawArea.Bottom - (float)(disk.CenterPoint.Y + disk.Radius) * scale;
            var r = disk.Radius * 2 * scale;

            graphics.DrawEllipse(pen, x, y, r, r);
        }
    }
}
