using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega.Ai
{
    interface IEnemyController
    {
        void Update(GameTime gameTime, List<Vector2> targets);
        
    }
}
