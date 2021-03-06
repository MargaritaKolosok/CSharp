﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 
 * для всех вариантов задач создать класс с указанными двумя
полями (Поле 1, Поле 2) и тремя методами:
- конструктор для инициализации объекта;
- функция формирования строки с информацией об объекте;
- функция обработки значений полей по индивидуальному варианту.
В основной программе вводить значения полей каждого объекта из компонентов
Edit и выводить результаты в компонент Memo.
Калорийность 100г
продукта
Вес продукта в
граммах
Вычислить общую калорийность
продукта 

создать класс-потомок с дополнительным полем, указанным в
индивидуальном задании (табл. 11.4, столб 2).
Реализовать в классе-потомке методы:
- конструктор;
- функцию обработки данных, указанную в индивидуальном задании (табл
11.4, столб 3).
 Создать проект для демонстрации работы: ввод и вывод информации об
объектах: классе-родителе и классе-потомке.

Количество витамина С в 1
грамме продукта
Количество витамина С в продукте
 
     */
class Product
{
    protected double weight;
    protected int callories;
    public string Name { get; set; }
    public double W
    {
        set => weight = value;
       
    }
    public int C
    {
        set => callories = value;
    }
    public Product(string name, double weight, int callories)
    {
        Name = name;
        this.weight = weight;
        this.callories = callories;
    }
    public double CountCallories()
    {
        return weight / 100 * callories;
    }
    public virtual void ShowInfo()
    {
        Console.WriteLine("Product {0} has {1} weight and {2} callories.", Name, weight, CountCallories()  );
    }
    public Product()
    {
    }
}
class Vitamin : Product
{
    public double VitaminC {get; set;}
       

    public Vitamin(string name, double weight, int callories, double vitaminC)
        :base(name,weight,callories)
    {
        VitaminC = vitaminC;
        Name = name;
        this.weight = weight;        
        this.callories = callories;
    }
    private double CountVitamin()
    {
        return weight / 1000 * VitaminC;
    }
    public override void ShowInfo()
    {
        base.ShowInfo();
        Console.WriteLine("Product {0} has Vitamins {1}.", Name, CountVitamin());
    }
}
namespace _1_CountCallories
{
    class Program
    {
        static void Main(string[] args)
        {
            Product Milk = new Product("Milk", 200, 34);
            Console.WriteLine(Milk.CountCallories());
            Product Bread = new Product();
            Bread.Name = "Baton";
            Bread.W = 300;
            Bread.C = 87;
            Console.WriteLine(Bread.CountCallories());

            Bread.ShowInfo();
            Milk.ShowInfo();

            Vitamin Lemon = new Vitamin("Lemon", 240, 66, 0.45);
            Console.WriteLine(Lemon.CountCallories()); 
            Lemon.ShowInfo();

            Console.ReadKey();
        }
    }
}
