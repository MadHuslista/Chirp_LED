# -*- coding: utf-8 -*-
"""
Created on Mon Nov 22 15:10:16 2021

@author: Meyerhof
"""

import numpy as np 
import matplotlib.pyplot as plt

save_path = r"D:\Proyecto_CHIRP_LED\Interface_NI6001\logs\calib_2.txt"


duration = 50
n_points = 5000 #step 1mA from  0 - 10V

#El delta está en 60.09mV 60.100mV y 60mV (orden del 00.01mV => 100.000 puntos en 10V)
period = duration/n_points

min_amp  = 0    #0 V
max_amp  = 10   #10V

sample_rate = 5000

steps = np.linspace(min_amp, max_amp, n_points+1, endpoint = True)

sig = np.array([])
for step in steps: 
    period_sig = step * np.ones(int(period*sample_rate))
    sig = np.concatenate((sig, period_sig))

sync = np.concatenate((np.zeros(sample_rate),np.ones(sample_rate)))    

sig = np.concatenate((sync, sig, sync))
print(len(sig))
#np.savetxt(save_path, sig, fmt='%.6f')
#plt.scatter(list(range(len(sig))),sig)

