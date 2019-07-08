using System;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Jitter.LinearMath;
using Jitter.Dynamics;
using Jitter.Collision.Shapes;
using Rhino.Input;
using Rhino.Input.Custom;
using System.Drawing;
using Rhino.UI;

namespace RigidBodyRhino
{
    public class JitterCylinder : Command
    {
        static JitterCylinder _instance;
        public JitterCylinder()
        {
            _instance = this;
        }

        ///<summary>The only instance of the MyCommand1 command.</summary>
        public static JitterCylinder Instance
        {
            get { return _instance; }
        }

        public override string EnglishName
        {
            get { return "JCylinder"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            if (TimePanel.Instance.Restarted && TimePanel.Instance.TrackbarValue == 0)
            {
                RhinoApp.WriteLine("Now it's time to draw a cylinder");

                Point3d pt0;
                using (GetPoint getPointAction = new GetPoint())
                {
                    getPointAction.SetCommandPrompt("The center of the base");
                    if (getPointAction.Get() != GetResult.Point) //getPointAction.Get() rimane in attesa del punto
                    {
                        RhinoApp.WriteLine("No center point was selected.");
                        return getPointAction.CommandResult();
                    }
                    pt0 = getPointAction.Point();
                    if (pt0.Z != 0)
                    {
                        RhinoApp.WriteLine("The center of the cylinder is not on the plane XY");
                        return getPointAction.CommandResult();
                    }
                }

                Point3d pt1;
                using (GetPoint getPointAction = new GetPoint())
                {
                    getPointAction.SetCommandPrompt("The end point of the radius of the cylinder");

                    getPointAction.SetBasePoint(pt0, true);
                    var line = new Line(pt0, new Point3d(pt0.X, pt0.Y, 1));
                    getPointAction.Constrain(line);
                    getPointAction.DynamicDraw +=
                                (sender, e) =>
                                {   
                                    e.Display.DrawLine(new Line(e.CurrentPoint, new Point3d(e.CurrentPoint.X, e.CurrentPoint.Y, -e.CurrentPoint.Z)), Color.Black);
                                };

                    if (getPointAction.Get() != GetResult.Point) //getPointAction.Get() rimane in attesa del punto
                    {
                        RhinoApp.WriteLine("No radius point was selected.");
                        return getPointAction.CommandResult();
                    }
                    pt1 = getPointAction.Point();

                }

                Point3d pt2;
                using (GetPoint getPointAction = new GetPoint())
                {
                    getPointAction.SetCommandPrompt("Height");

                    getPointAction.SetBasePoint(pt1, true);
                    var line = new Rhino.Geometry.Line(pt1, new Point3d(pt1.X, 1, pt1.Z));
                    getPointAction.Constrain(line);

                    getPointAction.DynamicDraw +=
                                (sender, e) =>
                                {
                                    e.Display.DrawLine(new Line(pt1, new Point3d(pt1.X, pt1.Y, -pt1.Z)), Color.Black);
                                    e.Display.DrawLine(new Line(pt1, e.CurrentPoint), Color.Black);
                                    e.Display.DrawLine(new Line(new Point3d(e.CurrentPoint.X, e.CurrentPoint.Y, -e.CurrentPoint.Z), e.CurrentPoint), Color.Black);
                                };
                    if (getPointAction.Get() != GetResult.Point) //getPointAction.Get() rimane in attesa del punto
                    {
                        RhinoApp.WriteLine("No Height point was selected.");
                        return getPointAction.CommandResult();
                    }
                    pt2 = getPointAction.Point();
                    if (pt2.Z == 0)
                    {
                        RhinoApp.WriteLine("The height of the cylinder must be different of 0");
                        return getPointAction.CommandResult();
                    }
                }

                double radius = new Line(pt0, pt1).Length;
                double height = new Line(pt1, pt2).Length;
                Shape cylinderShape = new CylinderShape((float) height, (float) radius);
                RigidBody rigidCylinder = new RigidBody(cylinderShape);
                //Translate to the user position
                rigidCylinder.Position = new JVector((float)(pt0.X), (float)(pt0.Y- height / 2), 0);

                Cylinder cylinder = new Cylinder(new Circle(Point3d.Origin, radius), height);

                //Original one with the center in 0,0,0  
                Cylinder copyCylinder = new Cylinder(new Circle(new Point3d(0, 0, 0), radius), height);
                Brep brepCylinder = copyCylinder.ToBrep(true, true);
                Transform trafo = MatrixXRotation(90);
                trafo = trafo.Transpose();
                //put center in 0,0,0
                brepCylinder.Translate(new Vector3d(0, 0, -height / 2));
                brepCylinder.Transform(trafo);
               
                //Copy to translate and rotate
                Brep copyToAdd = cylinder.ToBrep(true, true);
                copyToAdd.Translate(new Vector3d(0, 0, -height / 2));
                copyToAdd.Transform(trafo);
                if(pt0.Y>pt2.Y)
                    copyToAdd.Translate(new Vector3d(pt0.X, pt0.Y - height / 2, 0));
                else
                    copyToAdd.Translate(new Vector3d(pt2.X, pt2.Y - height / 2, 0));
                RigidBodyManager.RigidBodies.Add(rigidCylinder);
                RigidBodyManager.GeometryList.Add(brepCylinder);
                RigidBodyManager.GuidList.Add(doc.Objects.Add(copyToAdd));

                doc.Views.Redraw();
            }
            else
            {
                Dialogs.ShowMessage("Press Restart before use other commands", "Warning", ShowMessageButton.OK, ShowMessageIcon.Warning);
            }

            return Result.Success;
        }

        public static Transform MatrixXRotation(double degree)
        {
            double radians=degree*Math.PI / 180;
            Transform trafo = Transform.Identity;
            trafo.M11 = Math.Cos(radians);
            trafo.M12 = -Math.Sin(radians);

            trafo.M21 = Math.Sin(radians);
            trafo.M22 = Math.Cos(radians);

            return trafo;
        }
    }
}