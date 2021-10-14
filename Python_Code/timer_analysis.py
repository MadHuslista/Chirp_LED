# -*- coding: utf-8 -*-
"""
Created on Fri Oct  1 05:01:40 2021

@author: Meyerhof
"""

import numpy as np 
import matplotlib.pyplot as plt 
plt.close('all')

#Parámeters
Hz = 120 
duration = 28 #seg
period = round(1000/Hz,4) #[ms]


#data = np.loadtxt(r"D:\Proyecto_CHIRP_LED\Legacy_Logs\registro-120hz-30min.txt")
#data = np.loadtxt(r"D:\Proyecto_CHIRP_LED\Legacy_Logs\registro-60hz-15min.txt")

#data = np.loadtxt(r"D:\Proyecto_CHIRP_LED\Interface_NIDAQ\logs\pretime_registro.txt")
data = np.loadtxt(r"D:\Proyecto_CHIRP_LED\Interface_NIDAQ\logs\time_registro.txt")


intervals = np.diff(np.concatenate(([0],data)))

expected_data = np.linspace(period, 1000*duration, Hz*duration)

err = expected_data - data

#plt.plot(data, err, label = 'cum-err', color = 'blue')
plt.plot(data, intervals, label = ' interval', color = 'orange')
plt.xlabel('samples')
plt.ylabel('ms')
for i in range(1,4): 
    plt.hlines(intervals.mean() + i*intervals.std(), data[0], data[-1], color = 'c')
    plt.hlines(intervals.mean() - i*intervals.std(), data[0], data[-1], color = 'c')
plt.legend()
#plt.scatter(data, intervals)
plt.figure()
plt.hist(intervals, bins = 100)
plt.xlabel('ms')
plt.ylabel('samples')

print('Mean:{}\nStd: {}'.format(intervals.mean(), intervals.std()))

out_1ds = sum(abs(intervals - intervals.mean()) > intervals.std())
out_2ds = sum(abs(intervals - intervals.mean()) > 2*intervals.std())
out_3ds = sum(abs(intervals - intervals.mean()) > 3*intervals.std())
print('Elem sobre 1std: {} de {} - {}%'.format(out_1ds, len(intervals), round(100* out_1ds/len(intervals),3)))
print('Elem sobre 2std: {} de {} - {}%'.format(out_2ds, len(intervals), round(100* out_2ds/len(intervals),3)))
print('Elem sobre 3std: {} de {} - {}%'.format(out_3ds, len(intervals), round(100* out_3ds/len(intervals),3)))
print('Max Out from mean: {}[ms]'.format(max(intervals - intervals.mean())))
print('Diff w/expected end: {}[ms]'.format(err[-1]))
print('Diff by interval: {}[ms]'.format(err[-1]/len(data)))