using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RectO
{
    public class ImageDataManagementController
    {
        int maxDepth;
        List<List<LayerInfo>> backStack;
        List<List<LayerInfo>> forwardStack;

        static ImageDataManagementController()
        {
            var backGesture = new InputGestureCollection();
            backGesture.Add(new KeyGesture(Key.Z, ModifierKeys.Control));
            Back = new RoutedUICommand("Назад", "Back", typeof(MainWindow), backGesture);
            var forwardGesture = new InputGestureCollection();
            forwardGesture.Add(new KeyGesture(Key.Y, ModifierKeys.Control));
            Forward = new RoutedUICommand("Вперед", "Forward", typeof(MainWindow), forwardGesture);
        }

        public static RoutedUICommand Back
        { get; set; }
        public static RoutedUICommand Forward
        { get; set; }

        public ImageDataManagementController()
        {
            maxDepth = 10;
            backStack = new List<List<LayerInfo>>(maxDepth);
            forwardStack = new List<List<LayerInfo>>(maxDepth);
        }

        public ImageDataManagementController(int depth)
        {
            maxDepth = depth;
            backStack = new List<List<LayerInfo>>(maxDepth);
            forwardStack = new List<List<LayerInfo>>(maxDepth);
        }

        public bool CanBack()
        {
            return backStack.Count != 0;
        }

        public List<LayerInfo> GoBack(List<LayerInfo> currentFrame)
        {
            if (backStack.Count == 0)
                return null;
            if (forwardStack.Count == maxDepth)
                forwardStack.RemoveAt(0);
            forwardStack.Add(currentFrame);
            var prevBitmap = backStack[backStack.Count - 1];
            backStack.RemoveAt(backStack.Count - 1);
            return prevBitmap;
        }

        public bool CanForward()
        {
            return forwardStack.Count != 0;
        }

        public List<LayerInfo> GoForward(List<LayerInfo> currentFrame)
        {
            if (forwardStack.Count == 0)
                return null;
            if (backStack.Count == maxDepth)
                backStack.RemoveAt(0);
            backStack.Add(currentFrame);
            var nextBitmap = forwardStack[forwardStack.Count - 1];
            forwardStack.RemoveAt(forwardStack.Count - 1);
            return nextBitmap;
        }

        public void Next(List<LayerInfo> currentFrame)
        {
            while (forwardStack.Count != 0)
            {
                if (backStack.Count == maxDepth)
                    backStack.RemoveAt(0);
                backStack.Add(forwardStack[0]);
                forwardStack.RemoveAt(0);
            }
            if (backStack.Count == maxDepth)
                backStack.RemoveAt(0);
            backStack.Add(currentFrame);
        }

    }
}
