# -*- coding: utf-8 -*-
"""
Created on Mon Nov 22 14:56:06 2021

@author: Meyerhof
"""
import numpy as np
import pandas as pd 
import matplotlib.pyplot as plt 
plt.close('all')


path = r"D:\Proyecto_CHIRP_LED\Interface_NI6001\logs\rec_calib_3.csv"
raw_data = pd.read_csv(path, sep = ';', header =3)

data = raw_data['V']
time = raw_data['Unit']

plt.plot(time, data)
