# -*- coding: utf-8 -*-
"""
Created on Mon Nov  8 19:41:44 2021

@author: Meyerhof
"""

import numpy as np
import pandas as pd 
import matplotlib.pyplot as plt 
plt.close('all')


path = r"D:\Proyecto_CHIRP_LED\Interface_NI6001\logs\rec_light_chirp.csv"
ref = r"D:\Proyecto_CHIRP_LED\Interface_NI6001\logs\signal.txt"

sample_rate = 5000
period = 1/sample_rate

raw_data = pd.read_csv(path, sep = ';', header =3)
raw_data['V'] = raw_data['V'] - 0.02 #sensor bias (19mV)
ref_data = np.loadtxt(ref)/9 * 1.5785

raw_starttime = np.array(raw_data.loc[raw_data['V'] > 1.5, 'Unit'])[0]

duration = len(ref_data)*period
ref_time = np.linspace(raw_starttime, duration - period +raw_starttime, len(ref_data))

plt.plot(ref_time, ref_data)
plt.plot(raw_data['Unit'],raw_data['V'], color = 'g')