# -*- coding: utf-8 -*-
"""
Created on Mon Nov 29 05:32:03 2021

@author: Meyerhof
"""

import numpy as np 
import matplotlib.pyplot as plt 

plt.close('all')
#path_ref = r"D:\Proyecto_CHIRP_LED\Calibration_Test\logs\calib_arr4.txt"
#data = np.loadtxt(path_ref, delimiter=',')

#path = r"D:\Proyecto_CHIRP_LED\Calibration_Test\logs\calib_arr.txt"
path = r"D:\Proyecto_CHIRP_LED\Interface_Prototype\log\calib_arr.txt"
data = np.loadtxt(path, delimiter=',')

x = data[:,0]
y = data[:,1]

#plt.plot(x,x, c= 'r')
#plt.plot(x,x/2, c= 'r')
#plt.plot(x,y, c= 'r')
#plt.yscale('symlog')
#plt.xscale('symlog')


plt.grid(True, which='both')
plt.minorticks_on()
plt.scatter(x,y,s = 1, c= 'b')
#plt.yscale('log')
#plt.xscale('log')
#plt.scatter(data_v[:,0],data_v[:,1],s=1, c = 'g')