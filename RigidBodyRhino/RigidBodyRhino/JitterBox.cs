using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Jitter;
using Jitter.Collision;
using Jitter.LinearMath;
using Jitter.Dynamics;
using Jitter.Collision.Shapes;
using Rhino.DocObjects;
using Rhino.Input;
using Rhino.Input.Custom;
using System.Drawing;
using Rhino.UI;

namespace RigidBodyRhino
{
    public class JitterBox : Command
    {
        static JitterBox _instance;
        public JitterBox()
        {
            _instance = this;
        }

        ///<summary>The only instance of the JitterBox command.</summary>
        public static JitterBox Instance
        {
            get { return _instance; }
        }

        public override string EnglishName
        {
            get { return "JBox"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            if (TimePanel.Instance.Restarted && TimePanel.Instance.TrackbarValue == 0)
            {

                RhinoApp.WriteLine("Now it's time to draw a box");

                Point3d pt0;
                using (GetPoint getPointAction = new GetPoint())
                {
                    getPointAction.SetCommandPrompt("First corner of the base");
                    if (getPointAction.Get() != GetResult.Point) //getPointAction.Get() rimane in attesa del punto
                    {
                        RhinoApp.WriteLine("No corner point was selected.");
                        return getPointAction.CommandResult();
                    }
                    pt0 = getPointAction.Point();
                    if (pt0.Z != 0)
                    {
                        RhinoApp.WriteLine("The base of the square is not on the plane XY");
                        return getPointAction.CommandResult();
                    }
                }

                Point3d pt1;
                using (GetPoint getPointAction = new GetPoint())
                {
                    getPointAction.SetCommandPrompt("Other corner of the base");

                    getPointAction.SetBasePoint(pt0, true);

                    getPointAction.DynamicDraw +=
                                (sender, e) =>
                                {
                                    e.Display.DrawLine(pt0, new Point3d(pt0.X, e.CurrentPoint.Y, 0), Color.Black);
                                    e.Display.DrawLine(pt0, new Point3d(e.CurrentPoint.X, pt0.Y, 0), Color.Black);
                                    e.Display.DrawLine(new Point3d(pt0.X, e.CurrentPoint.Y, 0), e.CurrentPoint, Color.Black);
                                    e.Display.DrawLine(new Point3d(e.CurrentPoint.X, pt0.Y, 0), e.CurrentPoint, Color.Black);
                                };

                    if (getPointAction.Get() != GetResult.Point) //getPointAction.Get() rimane in attesa del punto
                    {
                        RhinoApp.WriteLine("No corner point was selected.");
                        return getPointAction.CommandResult();
                    }
                    pt1 = getPointAction.Point();
                    if (pt1.Z != 0)
                    {
                        RhinoApp.WriteLine("The base of the square is not on the plane XY");
                        return getPointAction.CommandResult();
                    }
                    if (pt1.Equals(pt0))
                    {
                        RhinoApp.WriteLine("The second point is the same of the first");
                        return getPointAction.CommandResult();
                    }
                }

                Point3d pt2;
                using (GetPoint getPointAction = new GetPoint())
                {
                    getPointAction.SetCommandPrompt("Height");

                    getPointAction.SetBasePoint(pt1, true);
                    var line = new Rhino.Geometry.Line(pt1, new Point3d(pt1.X, pt1.Y, 1));
                    getPointAction.Constrain(line);

                    getPointAction.DynamicDraw +=
                                (sender, e) =>
                                {
                                    e.Display.DrawLine(pt0, new Point3d(pt0.X, pt1.Y, 0), Color.Black);
                                    e.Display.DrawLine(pt0, new Point3d(pt1.X, pt0.Y, 0), Color.Black);
                                    e.Display.DrawLine(new Point3d(pt0.X, pt1.Y, 0), pt1, Color.Black);
                                    e.Display.DrawLine(new Point3d(pt1.X, pt0.Y, 0), pt1, Color.Black);

                                    e.Display.DrawLine(new Point3d(pt0.X, pt0.Y, e.CurrentPoint.Z), new Point3d(pt0.X, pt1.Y, e.CurrentPoint.Z), Color.Black);
                                    e.Display.DrawLine(new Point3d(pt0.X, pt0.Y, e.CurrentPoint.Z), new Point3d(pt1.X, pt0.Y, e.CurrentPoint.Z), Color.Black);
                                    e.Display.DrawLine(new Point3d(pt0.X, pt1.Y, e.CurrentPoint.Z), e.CurrentPoint, Color.Black);
                                    e.Display.DrawLine(new Point3d(pt1.X, pt0.Y, e.CurrentPoint.Z), e.CurrentPoint, Color.Black);

                                    e.Display.DrawLine(pt0, new Point3d(pt0.X, pt0.Y, e.CurrentPoint.Z), Color.Black);
                                    e.Display.DrawLine(pt1, new Point3d(pt1.X, pt1.Y, e.CurrentPoint.Z), Color.Black);
                                    e.Display.DrawLine(new Point3d(pt0.X, pt1.Y, e.CurrentPoint.Z), new Point3d(pt0.X, pt1.Y, 0), Color.Black);
                                    e.Display.DrawLine(new Point3d(pt1.X, pt0.Y, e.CurrentPoint.Z), new Point3d(pt1.X, pt0.Y, 0), Color.Black);

                                };
                    if (getPointAction.Get() != GetResult.Point) //getPointAction.Get() rimane in attesa del punto
                    {
                        RhinoApp.WriteLine("No Height point was selected.");
                        return getPointAction.CommandResult();
                    }
                    pt2 = getPointAction.Point();
                    if (pt2.Z == 0)
                    {
                        RhinoApp.WriteLine("The height of the box must be different of 0");
                        return getPointAction.CommandResult();
                    }
                }

                //Find center of the box
                Point3d middleDiagonal = new Point3d((pt0.X + pt1.X) / 2, (pt0.Y + pt1.Y) / 2, 0);
                Point3d middleHeight = new Point3d(0, 0, (pt1.Z + pt2.Z) / 2);
                Point3d centerBox = new Point3d(middleDiagonal.X, middleDiagonal.Y, middleHeight.Z);
                //Find dimension of the box
                Shape boxShape = new BoxShape((float)Math.Abs(pt1.X - pt0.X), (float)Math.Abs(pt1.Y - pt0.Y), (float)Math.Abs(pt2.Z));
                RigidBody rigidBox = new RigidBody(boxShape);
                rigidBox.Position = new JVector((float)centerBox.X, (float)centerBox.Y, (float)centerBox.Z);

                Box box = new Box(new BoundingBox(RigidBodyManager.JVectorToPoint3d(boxShape.BoundingBox.Min), RigidBodyManager.JVectorToPoint3d(boxShape.BoundingBox.Max)));
                
                
                //Original one with the center in 0,0,0
                Brep brepBox = box.ToBrep();
                //Copy to translate and rotate
                Brep copyToAdd = brepBox.DuplicateBrep();
                //Move the box to the correct position
                copyToAdd.Translate(centerBox.X, centerBox.Y, centerBox.Z);

                RigidBodyManager.RigidBodies.Add(rigidBox);
                RigidBodyManager.GeometryList.Add(brepBox);
                RigidBodyManager.GuidList.Add(doc.Objects.Add(copyToAdd));

                doc.Views.Redraw();

                return Result.Success;
            }
            else
            {
                Dialogs.ShowMessage("Press Restart before use other commands", "Warning", ShowMessageButton.OK, ShowMessageIcon.Warning);
                return Result.Success;
            }
        }
    }
}