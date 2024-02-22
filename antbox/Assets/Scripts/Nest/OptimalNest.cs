using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
public class OptimalNest{
        public int enemyNumber;
        public NestType nestType;
        public int maxEnemyLevel=1;

        public OptimalNest(int enemy,NestType type,int max){
            enemyNumber=enemy;
            nestType=type;
            maxEnemyLevel=max;
        }
}
