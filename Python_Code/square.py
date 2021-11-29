# -*- coding: utf-8 -*-
"""
Created on Thu Nov 11 10:30:54 2021

@author: Meyerhof
"""

import numpy as np 
from scipy.signal import square as square_sig
import matplotlib.pyplot as plt 

save_path = r"D:\Proyecto_CHIRP_LED\Interface_NI6001\logs\square.txt"

duration_s = 10
sample_rate = 5000

square_freq = 5
amplitud = 9#V

t = np.linspace(0, duration_s, sample_rate * duration_s, endpoint = False)

sig = amplitud * square_sig(2 * np.pi * t * square_freq)

#plt.plot(t, sig)

np.savetxt(save_path, sig, fmt='%.6f')