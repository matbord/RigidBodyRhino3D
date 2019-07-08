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

    public class RigidBodyRhinoCommand : Command
    {

        public RigidBodyRhinoCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;

        }

        ///<summary>The only instance of this command.</summary>
        public static RigidBodyRhinoCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "RigidBody"; }
        }

        private World world;

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            if (TimePanel.Instance.Restarted && TimePanel.Instance.TrackbarValue == 0)
            {
                world = new World(new CollisionSystemSAP());
                for (int i = 0; i < RigidBodyManager.GuidList.Count; i++)
                {
                    Guid currentGuid = RigidBodyManager.GuidList[i];
                    RhinoObject rhinoObject = doc.Objects.FindId(currentGuid);

                    RigidBody currentRigidBody = RigidBodyManager.RigidBodies[i];
                    world.AddBody(currentRigidBody);
                }
                RigidBodyManager.World = world;

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