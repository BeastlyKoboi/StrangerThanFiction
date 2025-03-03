using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CardPile
{
    public List<CardModel> cards = new List<CardModel>();

    public event Action OnChange;
    public event Action<CardModel> OnCardAdded;
    public event Action<CardModel> OnCardRemoved;
    public event Action OnShuffle;
    public CardModel this[int index]
    {
        // get and set accessors
        get => cards[index];
        set => cards[index] = value;
    }

    public int Count 
    {
        get { return cards.Count; }
    }

    public CardModel[] ToArray()
    {
        return cards.ToArray();
    }

    public void ForEach(Action<CardModel> action)
    {
        cards.ForEach(action);
    }
    public async UniTask ForEach(Func<CardModel, UniTask> action)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            await action(cards[i]);
        }
    }

    public void Add(CardModel card)
    {
        cards.Add(card);

        OnChange?.Invoke();
        OnCardAdded?.Invoke(card);
    }
    public void Insert(int index, CardModel card)
    {
        cards.Insert(index, card);

        OnChange?.Invoke();
        OnCardAdded?.Invoke(card);
    }
    public void Remove(CardModel card)
    {
        cards.Remove(card);

        OnChange?.Invoke();
        OnCardRemoved?.Invoke(card);
    }
    public void RemoveAt(int index)
    {
        CardModel card = cards[index];
        cards.RemoveAt(index);

        OnChange?.Invoke();
        OnCardRemoved?.Invoke(card);
    }
    public void Clear()
    {
        cards.Clear();

        OnChange?.Invoke();
        //OnCardRemoved?.Invoke();
    }
    public void Shuffle()
    {

        int n = cards.Count;
        System.Random rng = new System.Random();
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            CardModel value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }

        OnShuffle?.Invoke();
    }


}
