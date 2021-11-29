# -*- coding: utf-8 -*-
"""
Created on Mon Nov 22 15:45:30 2021

@author: Meyerhof
"""

import numpy as np
import pandas as pd 
import matplotlib.pyplot as plt 
plt.close('all')


path = r"D:\Proyecto_CHIRP_LED\Interface_NI6001\logs\rec_calib_2.csv"
orig_path = r"D:\Proyecto_CHIRP_LED\Interface_NI6001\logs\calib_2.txt"

raw_data = pd.read_csv(path, sep = ';', header =3)
ref_data = np.loadtxt(orig_path) / 8

sample_rate = 5000
period = 1/sample_rate
start_ref = 2

raw_starttime = np.array(raw_data.loc[raw_data['V'] < start_ref, 'Unit'])[0]
start_loc = raw_data.loc[raw_data['V'] < start_ref, 'Unit'].index[0]

duration = len(ref_data)*period
ref_time = np.linspace(raw_starttime, duration - period +raw_starttime, len(ref_data))

data = raw_data['V'] 
time = raw_data['Unit'] * 1/ (2.264-0.359)

plt.scatter(ref_time, ref_data)
plt.scatter(time[start_loc:], data[start_loc:])
#plt.plot(time, ori_data)