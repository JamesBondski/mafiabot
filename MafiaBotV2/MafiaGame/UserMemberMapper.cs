using System;
using System.Collections.Generic;
using System.Text;
using MafiaBotV2.Network;
using MafiaBotV2.MafiaLib;

namespace MafiaBotV2.MafiaGame
{
    public class Mapper<T, U>
    {
        Dictionary<T, U> userToVillage = new Dictionary<T, U>();
        Dictionary<U, T> villageToUser = new Dictionary<U, T>();

        public void Add(T user, U member) {
            userToVillage.Add(user, member);
            villageToUser.Add(member, user);
        }

        public U Remove(T user) {
            U member = userToVillage[user];
            userToVillage.Remove(user);
            villageToUser.Remove(member);
            return member;
        }

        public T Remove(U member) {
            T user = villageToUser[member];
            userToVillage.Remove(user);
            villageToUser.Remove(member);
            return user;
        }

        public T this[U member] {
            get { if(!villageToUser.ContainsKey(member)) return default(T); else return villageToUser[member]; }
        }

        public U this[T user] {
            get { if (!userToVillage.ContainsKey(user)) return default(U); else return userToVillage[user]; }
        }
    }
}
