using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace StructuralPlaneStatistics.Classes
{
    public class CornerCircle
    {
        public float Current_X;
        public float Current_Y;
        public float Previous_X;
        public float Previous_Y;
        private static Random rnd = new Random();
        private int diameter; //圆的直径
        enum State { In, Out } //触摸点是否在圆内
        State state = State.Out;
        Paint fill; //填充颜色画笔
        Paint line; //画线画笔

        public CornerCircle()
        {
            this.diameter = 50;
            fill = new Paint() { Color = Color.Yellow, Alpha=128, AntiAlias = true };
            line = new Paint() { Color = Color.Black, StrokeWidth = 5, AntiAlias = true };
            line.SetStyle(Paint.Style.Stroke);
        }

        public CornerCircle(float x, float y)
        {
            Current_X = x;
            Current_Y = y;
            this.diameter = 50;
            fill = new Paint() { Color = Color.Yellow, Alpha = 128, AntiAlias = true };
            line = new Paint() { Color = Color.Black, StrokeWidth = 5, AntiAlias = true };
            line.SetStyle(Paint.Style.Stroke);
        }

        public void Draw(Android.Graphics.Canvas canvas)
        {
            canvas.DrawCircle(Current_X, Current_Y, diameter, line);
            canvas.DrawCircle(Current_X, Current_Y, diameter, fill);
        }

        public bool Move(float x, float y)
        {
            if (state == State.In)
            {
                Current_X += x;
                Current_Y += y;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SetState(float x, float y, MotionEventActions mea)
        {
            if (Math.Sqrt((Current_X - x) * (Current_X - x) + (Current_Y - y) * (Current_Y - y)) < diameter)
            {
                if (MotionEventActions.Pointer2Down == mea)
                {
                    return true;
                }

                if (mea == MotionEventActions.Down)
                {
                    state = State.In;
                }
                else
                {
                    state = State.Out;
                }
                return true;
            }
            else
            {
                state = State.Out;
                return false;
            }
        }
    }
}