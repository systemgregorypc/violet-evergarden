__author__ = 'jose gregorio'

from random import random, randint
import numpy as np
import math
import decimal


class Individuos(object):

    def generarIndividuos(self, cantidad, elementos):

        IndivTotal = []
        cantidadIndividuos = cantidad
        valorPorIndividuo = elementos
        for i in range(0, cantidadIndividuos):
            IndivTotal.append(self.generarIndividuo(valorPorIndividuo))
        IndivTotal = np.array(IndivTotal)
        print(IndivTotal)
        c = []
        for elemento in range(0, len(IndivTotal)):
            a = list(IndivTotal[elemento, :])
            b = []
            for ind in a:
                b.append(self.dec2bin(ind))
            c.append(b)
        c = np.array(c)
        print(c)
        z = []
        for elem in range(0, len(c)):
            x = list(c[elem, :])
            y = []
            for ind in x:
                y.append(self.bin2dec(ind))
            z.append(y)
        z = np.array(z)
        print(z)
        return IndivTotal, c, z

    def generarIndividuo(self, cantidad):

        indiv = []
        for i in range(0, cantidad):
            indiv.append(self.randBi(float(decimal.Decimal('%d.%d' % (randint(0,0),randint(0,9999))))))
        return indiv

    def randBi(self, randomNumber):
        factorInversor = random()
        factorMezcla = random()
        if factorInversor > factorMezcla:
            randomNumber = -randomNumber
        else:
            randomNumber = randomNumber

        return randomNumber

    def dec2bin(self,f):

        signo = ""
        if f<0:
            f = abs(f)
            signo = "1"

        else:
            signo = "0"

        if f >= 1:
            g = int(math.log(f, 2))
        else:
            g = -1
        h = g + 1
        ig = math.pow(2, g)
        st = ""
        while f > 0 or ig >= 1:
            if f < 1:
                if len(st[h:]) >= 10: # 10 fractional digits max
                       break
            if f >= ig:
                st += "1"
                f -= ig
            else:
                st += "0"
            ig /= 2
        st = signo + st[:h] + st[h:]
        return st

    def bin2dec(self, binario):

        if binario[0] == '1':
            flag = "Negative"
        else:
            flag = "Positive"

        j = 0
        valor = float(0.0)

        for i in range(1, len(binario)):
            j = i
            if binario[i] == "1":
                valor = valor + 1/float(pow(2, j))
            else:
                valor = valor + 0/float(pow(2, j))
        if flag == "Negative":
            valor = -valor
        valor=round(valor,4)
        return valor

genera = Individuos()
genera.generarIndividuos(10,4)
