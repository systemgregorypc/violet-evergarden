# -*- coding: utf-8 -*-
"""
Created on Sunday 09 October 19:58:30 2022
"""
from math import exp

class Neurona():
    def __init__(self):
        self.a = float(input("valor de bias: "))
        self.x = input("valores de entrada (con coma después del dato): ")
        self.w = input("valores de de pesos (con coma después del dato): ")

    def Sumatoria(self, x, w):
        Vk = 0

        for Entrada in range(0, len(x)):
            Vk = Vk + x[Entrada]*w[Entrada]
        return Vk

    def Sigmoide(self, a, Sumatoria):
        Sigmoid = 1/(1+exp(-a*Sumatoria))
        return Sigmoid
