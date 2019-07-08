using System;
using Rhino;
using Rhino.Commands;
using Rhino.UI;
using Rhino.DocObjects;
using Rhino.Geometry;
using Jitter.LinearMath;
using Jitter.Dynamics;
using System.Collections.Generic;
using Jitter.Collision.Shapes;

namespace RigidBodyRhino
{
    public class JCompoundShape : Command
    {
        static JCompoundShape _instance;
        public JCompoundShape()
        {
            _instance = this;
        }

        ///<summary>The only instance of the JCompoundShape command.</summary>
        public static JCompoundShape Instance
        {
            get { return _instance; }
        }

        public override string EnglishName
        {
            get { return "JCompoundShape"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            if (TimePanel.Instance.Restarted && TimePanel.Instance.TrackbarValue == 0)
            {

                ObjRef[] objrefs;
                Result rc = Rhino.Input.RhinoGet.GetMultipleObjects("Select the polysurfaces for the union",
                                                                    false, ObjectType.Brep, out objrefs);
                if (rc != Result.Success)
                    return rc;
                if (objrefs == null || objrefs.Length <= 1) { 
                    Dialogs.ShowMessage("You have to select 2 or more shapes.", "Warning", ShowMessageButton.OK, ShowMessageIcon.Warning);
                    return Result.Failure;
                }


                List<Brep> in_breps0 = new List<Brep>();
                for (int i = 0; i < objrefs.Length; i++)
                {
                    int index = RigidBodyManager.GuidList.IndexOf(objrefs[i].ObjectId);
                    //Avoid to create a compound from another compound
                    if (RigidBodyManager.RigidBodies[index].Shape is CompoundShape)
                    {
                        Dialogs.ShowMessage("You cannot create compound shape from another compound shape. Try to create it at once.", "Warning", ShowMessageButton.OK, ShowMessageIcon.Warning);
                        return Result.Failure;
                    }
                    //Accept shapes only if they intersect to each other
                    Brep brep = objrefs[i].Brep();
                    if (brep != null)
                        in_breps0.Add(brep);
                    RhinoDoc.ActiveDoc.Objects.Delete(objrefs[i], true);
                }

                //Create the rhino compound shape
                double tolerance = doc.ModelAbsoluteTolerance;
                Brep[] breps = Brep.CreateBooleanUnion(in_breps0, tolerance);
                if (breps.Length > 1)
                {
                    Dialogs.ShowMessage("You cannot create more than a compound shape in once time.", "Warning", ShowMessageButton.OK, ShowMessageIcon.Warning);
                    return Result.Failure;
                }

                Brep rhinoCompound = breps[0];
                // If the user create zero or more than 1 compound the command fails
                if (breps.Length != 1)
                    return Rhino.Commands.Result.Nothing;

                Brep copyToAdd = rhinoCompound.DuplicateBrep();

                //Create the rigid compound shape
                CompoundShape.TransformedShape[] transformedShapes = new CompoundShape.TransformedShape[in_breps0.Count];

                for (int i = 0; i < in_breps0.Count; i++)
                {
                    Guid guid = objrefs[i].ObjectId;
                    int indexRigidBody = RigidBodyManager.GuidList.IndexOf(guid);
                    transformedShapes[i] = new CompoundShape.TransformedShape(RigidBodyManager.RigidBodies[indexRigidBody].Shape, RigidBodyManager.RigidBodies[indexRigidBody].Orientation, RigidBodyManager.RigidBodies[indexRigidBody].Position);
                }
                CompoundShape jCompound = new CompoundShape(transformedShapes);
                RigidBody jCompoundBody = new RigidBody(jCompound);

                //Move the center of mass of Jitter shape on the center of the BBox of rhino shape
                Point3d centerBbox = rhinoCompound.GetBoundingBox(true).Center;
                jCompoundBody.Position = RigidBodyManager.Point3dtoJVector(centerBbox);
                //Find the difference between rhino bbx center and jitter bbox center
                JVector bboxjitter = jCompoundBody.BoundingBox.Center;
                JVector diff = bboxjitter - RigidBodyManager.Point3dtoJVector(centerBbox);
                //Align the center of both bboxes
                jCompoundBody.Position -= diff;

                //Translate the center of the Bbox to 0,0,0 and save it to geometry list
                Point3d bboxDoc = rhinoCompound.GetBoundingBox(true).Center;
                rhinoCompound.Translate(new Vector3d(-bboxDoc.X, -bboxDoc.Y, -bboxDoc.Z));

                RigidBodyManager.RigidBodies.Add(jCompoundBody);
                RigidBodyManager.GeometryList.Add(rhinoCompound);
                Guid guidToAdd = doc.Objects.Add(copyToAdd);
                RigidBodyManager.GuidList.Add(guidToAdd);

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