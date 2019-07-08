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
    public class JitterSphere : Command
    {
        static JitterSphere _instance;
        public JitterSphere()
        {
            _instance = this;
        }

        ///<summary>The only instance of the MyCommand1 command.</summary>
        public static JitterSphere Instance
        {
            get { return _instance; }
        }

        public override string EnglishName
        {
            get { return "JSphere"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            if (TimePanel.Instance.Restarted && TimePanel.Instance.TrackbarValue == 0) { 
                RhinoApp.WriteLine("Now it's time to draw a sphere");

                Point3d pt0;
                using (GetPoint getPointAction = new GetPoint())
                {
                    getPointAction.SetCommandPrompt("The center of the sphere");
                    if (getPointAction.Get() != GetResult.Point) //getPointAction.Get() rimane in attesa del punto
                    {
                        RhinoApp.WriteLine("No center point was selected.");
                        return getPointAction.CommandResult();
                    }
                    pt0 = getPointAction.Point();
                    if (pt0.Z != 0)
                    {
                        RhinoApp.WriteLine("The center of the sphere is not on the plane XY");
                        return getPointAction.CommandResult();
                    }
                }

                Point3d pt1;
                using (GetPoint getPointAction = new GetPoint())
                {
                    getPointAction.SetCommandPrompt("The end point of the radius of the sphere");

                    getPointAction.SetBasePoint(pt0, true);

                    getPointAction.DynamicDraw +=
                                (sender, e) =>
                                {
                                    e.Display.DrawSphere(new Sphere(pt0, new Line(pt0, e.CurrentPoint).Length), Color.Black);
                                };

                    if (getPointAction.Get() != GetResult.Point) //getPointAction.Get() rimane in attesa del punto
                    {
                        RhinoApp.WriteLine("No radius point was selected.");
                        return getPointAction.CommandResult();
                    }
                    pt1 = getPointAction.Point();

                }

                double radius = new Line(pt0, pt1).Length;

                Shape sphereShape = new SphereShape((float)radius);
                RigidBody rigidSphere = new RigidBody(sphereShape);
                
                rigidSphere.Position = new JVector((float)pt0.X, (float)pt0.Y, (float)pt0.Z);
                Sphere sphere = new Sphere(pt0, radius);
                //Original one with the center in 0,0,0
                Sphere copySphere = new Sphere(new Point3d(0,0,0), radius);
                Brep brepSphere = copySphere.ToBrep();
                //Copy to translate and rotate
                Brep copyToAdd = sphere.ToBrep();

                RigidBodyManager.RigidBodies.Add(rigidSphere);
                RigidBodyManager.GeometryList.Add(brepSphere);
                RigidBodyManager.GuidList.Add(doc.Objects.Add(copyToAdd));

                doc.Views.Redraw();
                return Result.Success;
            }
            else {
                Dialogs.ShowMessage("Press Restart before use other commands", "Warning", ShowMessageButton.OK, ShowMessageIcon.Warning);
                return Result.Success;
            }

        }
    }
}