# -*- coding: utf-8 -*-
"""
Created on Mon Nov  8 11:00:34 2021

@author: Meyerhof
"""

import numpy as np 
import matplotlib.pyplot as plt
from scipy import signal
plt.close('all')
#save_path = r"D:\Proyecto_CHIRP_LED\Interface_NI6001\logs\probe_signal.txt"
save_path = r"D:\Proyecto_CHIRP_LED\Calibration_Test\logs\probe_signal.txt"



sample_rate = 5000
period = 1/sample_rate
wave_freq = 100
amplitud = 3 #V


duration = 30

t = np.linspace(0, duration - period , sample_rate*duration)
sig = signal.sawtooth(2 * np.pi * wave_freq * t,0.5)/2 +0.5

sync = np.concatenate((np.zeros(sample_rate), np.ones(sample_rate)))

sig = np.concatenate((sync, sig))


sig = amplitud * sig + 0.1

#sig_diff = np.diff(sig)

plt.plot(sig)
#plt.hlines(0, t[0], t[-1])
#plt.figure()
#plt.scatter(t[:-1], np.diff(sig))

np.savetxt(save_path, sig, fmt='%.6f')