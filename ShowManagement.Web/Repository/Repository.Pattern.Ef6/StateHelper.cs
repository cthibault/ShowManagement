using Entities.Pattern;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Pattern.Ef6
{
    public class StateHelper
    {
        public static EntityState ConvertState(ObjectState state)
        {
            EntityState entityState = EntityState.Unchanged;

            switch (state)
            {
                case ObjectState.Added:
                    entityState = EntityState.Added;
                    break;

                case ObjectState.Modified:
                    entityState = EntityState.Modified;
                    break;

                case ObjectState.Deleted:
                    entityState = EntityState.Deleted;
                    break;

                default:
                    entityState = EntityState.Unchanged;
                    break;
            }

            return entityState;
        }

        public static ObjectState ConvertState(EntityState state)
        {
            ObjectState objectState = ObjectState.Unchanged;

            switch (state)
            {
                case EntityState.Added:
                    objectState = ObjectState.Added;
                    break;

                case EntityState.Deleted:
                    objectState = ObjectState.Deleted;
                    break;


                case EntityState.Modified:
                    objectState = ObjectState.Modified;
                    break;

                case EntityState.Detached:
                case EntityState.Unchanged:
                    objectState = ObjectState.Unchanged;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("state");
            }

            return objectState;
        }
    }
}
