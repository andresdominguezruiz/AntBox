using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CardResources
{
    [SerializeField] readonly int ALL_GOOD_CARDS=21;
    [SerializeField] readonly int ALL_BAD_CARDS=1;
    [Test]
    public void CardsSize()
    {
        Card[] allCards=Resources.LoadAll<Card>("Cards");
        Assert.That(allCards.Length==(ALL_BAD_CARDS+ALL_GOOD_CARDS));

    }
}
