__author__ = 'JGHC'
# -*- encoding: utf-8 -*-

from random import random, randrange
import numpy as np
import math

class algoritmoGenetico(object):

    def __init__(self):
        pass

    def generarPesosRandom(self, individuos):
        newPesos = []
        pesos = np.ones((individuos, individuos))
        for renglon in pesos:
            newrenglon = []
            for peso in renglon:
                newPeso = self.randBi(random())
                newrenglon.append(newPeso)
            newPesos.append(newrenglon)
        print(newrenglon)
        
         def seleccionNatural(self, individuos):
        #self.redNeural = Red_Neurona()
        pass

    def cruzaEnUnPunto(self, indiv1, indiv2):
        """
        :param indiv1: tipo "lista" Individuo padre/madre que proveerá de parte de sus genes
        :param indiv2: tipo "lista" Individuo padre/madre que proveerá parte de sus genes
        :return: Devuelve dos listas, una por descendiente resultante de la mezcla entre los genes de los progenitores
        """
        if len(indiv1) > len(indiv2):
            maxRange = len(indiv1) - 1
        else:
            maxRange = len(indiv2) - 1
        crossPoint = randrange(0, maxRange)
        alpha = random()
        umbral = 0.6
        if alpha > umbral:
            descendiente1 = indiv1[0:crossPoint] + indiv2[crossPoint:]
            descendiente2 = indiv2[0:crossPoint] + indiv1[crossPoint:]
        else:
            descendiente1 = indiv2[0:crossPoint] + indiv1[crossPoint:]
            descendiente2 = indiv1[0:crossPoint] + indiv2[crossPoint:]
        return descendiente1, descendiente2

    def cruzaVariable(self, indiv1, indiv2):
        """
        :param indiv1: tipo "lista" Individuo padre/madre que proveerá de parte de sus genes
        :param indiv2: tipo "lista" Individuo padre/madre que proveerá parte de sus genes
        :return: Devuelve dos listas, una por descendiente resultante de la mezcla entre los genes de los progenitores
        """
 
        if len(indiv1) > len(indiv2):
            maxRange = len(indiv1)
        else:
            maxRange = len(indiv2)
        crossPoint_1 = randrange(0, maxRange)
        crossPoint_2 = randrange(0, maxRange)
        if crossPoint_1 > crossPoint_2:
            valorLoco = crossPoint_1
            crossPoint_1 = crossPoint_2
            crossPoint_2 = valorLoco
        alpha = random()
        umbral = random()
        if alpha > umbral:
            descendiente1 = indiv2[:crossPoint_1] + indiv1[crossPoint_1:crossPoint_2] + indiv2[crossPoint_2:]
            descendiente2 = indiv1[:crossPoint_1] + indiv2[crossPoint_1:crossPoint_2] + indiv1[crossPoint_2:]

        else:
            descendiente2 = indiv2[:crossPoint_1] + indiv1[crossPoint_1:crossPoint_2] + indiv2[crossPoint_2:]
            descendiente1 = indiv1[:crossPoint_1] + indiv2[crossPoint_1:crossPoint_2] + indiv1[crossPoint_2:]

        return descendiente1, descendiente2

    def Mutacion(self, individuo):
        """
        :param individuo: Individuo Tipo Lista, binarizada, los datos binarios deben estar como string
        :return: individuo mutado o no mutado, resultante de pasar por el proceso aleatorio de mutación
        """
        individuo = str(individuo)
        probabilidad = 0.07
        genMutante = ''
        for i in range(0, len(individuo)):
            factorMutageno = random()
            if factorMutageno < probabilidad:
                if individuo[i] == '0':
                    genMutante = genMutante + '1'
                else:
                    genMutante = genMutante + '0'
            else:
                genMutante = genMutante + individuo[i]
        return genMutante

    def generarIndividuos(self, cantidad, elementos):
        """
        Este método genera una población inicial basado en una cantidad de individuos y un número de elementos
        :param cantidad: Tipo Int, número de individuos que conformarán la población inicial
        :param elementos: Tipo Int, número de propiedades que poseerá cada individuo
        :return: Matriz de población aleatoria
        """

        IndivTotal = []
        cantidadIndividuos = cantidad
        valorPorIndividuo = elementos
        for i in range(0, cantidadIndividuos):
            IndivTotal.append(self.generarIndividuo(valorPorIndividuo))
        IndivTotal = np.array(IndivTotal)
        return IndivTotal

    def crearFenotipos(self, IndivTotal):
        """
        :param IndivTotal: tipo "np.array" Conjunto de individuos cuyos genes serán convertidos a binario
        :return: Devuelve un "np.array" con el conjunto de individuos binarizados
        """
        c = []
        for elemento in range(0, len(IndivTotal)):
            a = list(IndivTotal[elemento, :])
            b = []
            for ind in a:
                b.append(self.dec2bin(ind))
            c.append(b)
        c = np.array(c)
        return c

    def crearGenotipos(self, fenotipo):
        z = []
        for elem in range(0, len(fenotipo)):
            x = list(fenotipo[elem, :])
            y = []
            for ind in x:
                y.append(self.bin2dec(ind))
            z.append(y)
        z = np.array(z)
        return z

    def generarIndividuo(self, cantidad):
        indiv = []
        for i in range(0, cantidad):
            indiv.append(self.randBi(random()))
        return indiv

    def randBi(self, randomNumber):
        """
        """
        factorInversor = random()
        factorMezcla = random()
        if factorInversor > factorMezcla:
            randomNumber = -randomNumber
        else:
            randomNumber = randomNumber
        randomNumber = round(randomNumber, 4)
        return randomNumber

    def dec2bin(self, f):
        signo = ""
        if f < 0:
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
                if len(st[h:]) >= 10:  # 10 fractional digits max
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
                valor = valor + 1 / float(pow(2, j))
            else:
                valor = valor + 0 / float(pow(2, j))
        if flag == "Negative":
            valor = -valor
        valor = round(valor, 4)
        return valor


genonet = algoritmoGenetico() libre albedrio ,
"""
indiv = genonet.generarIndividuos(10,6)
genoin = genonet.crearFenotipos(indiv)
print(indiv)
print(genoin)
"""
"""
papa = genonet.generarIndividuo(6)
mama = genonet.generarIndividuo(6)
print papa, mama
hijo, hija = genonet.cruzaVariable(papa, mama)
print(hijo, hija)
hijos = [hijo, hija]
hijos = np.array(hijos)
hijos = genonet.crearFenotipos(hijos)
hijo = hijos[0]
hija = hijos[1]
newHijo = []
for elemento in range(len(hijo)):
     newHijo.append(genonet.Mutacion(hijo[elemento]))
newHija = []
for elemento in range(len(hija)):
     newHija.append(genonet.Mutacion(hija[elemento]))
hijos = [newHijo, newHija]
hijos = np.array(hijos)
newHijos = genonet.crearGenotipos(hijos)
hijo = newHijos[0]
hija = newHijos[1]
print(hijo, hija)"""
genonet.generarPesosRandom(10)
