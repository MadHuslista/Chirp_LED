# -*- coding: utf-8 -*-
"""
Created on Sun Nov  7 11:29:37 2021

@author: Meyerhof
"""
import numpy as np
import pandas as pd 
import matplotlib.pyplot as plt 
plt.close('all')


path = r"D:\Proyecto_CHIRP_LED\Interface_NI6001\logs\rec_light_12.csv"

expected_slope = 0.04
#reset_slope = -1

raw_data = pd.read_csv(path, sep = ';', header =3)

#plt.scatter(raw_data['Unit'][:-1],np.diff(raw_data['V']), s = 1,c = 'r', label='prepros')
#plt.figure()
d = np.diff(raw_data['V'])
d = np.abs(d)
plt.plot(raw_data['Unit'][:-1],d, color = 'c')
plt.scatter(raw_data['Unit'][:-1],d, color = 'r', s= 1)
plt.plot(raw_data['Unit'],raw_data['V']/9, color = 'g')


diff_data = np.abs(np.diff(raw_data['V']))
diff_data = np.concatenate((diff_data, [0]))
#diff_data[diff_data < reset_slope] = expected_slope

diff_data[diff_data < expected_slope] = 0

raw_data['dV'] = diff_data

#plt.plot(raw_data['Unit'],raw_data['dV'], label='postpros')
#plt.legend()


time_points = np.array(raw_data.loc[raw_data['dV'] >= expected_slope, 'Unit'])
plt.vlines(time_points, 0,0.3)
periods = np.diff(time_points)


# 
plt.figure()                        
plt.plot(time_points[:-1], periods, label = 'periods', color = 'orange')
plt.xlabel('time [s]')
plt.ylabel('period [s]')
for i in range(1,4): 
    plt.hlines(periods.mean() + i*periods.std(), time_points[0], time_points[-1], color = 'c')
    plt.hlines(periods.mean() - i*periods.std(), time_points[0], time_points[-1], color = 'c')
#plt.legend()
#plt.scatter(time_points, periods)
plt.figure()
plt.hist(periods, bins = 100)
plt.xlabel('period [s]')
plt.ylabel('samples')


print('Mean:{}[s]\nStd: {}[s]\n\nMesured Sampled Rate: {}[Hz]\n'.format(periods.mean(), periods.std(),1/periods.mean()))

out_1ds = sum(abs(periods - periods.mean()) > periods.std())
out_2ds = sum(abs(periods - periods.mean()) > 2*periods.std())
out_3ds = sum(abs(periods - periods.mean()) > 3*periods.std())
print('Elem sobre 1std: {} de {} - {}%'.format(out_1ds, len(periods), round(100* out_1ds/len(periods),3)))
print('Elem sobre 2std: {} de {} - {}%'.format(out_2ds, len(periods), round(100* out_2ds/len(periods),3)))
print('Elem sobre 3std: {} de {} - {}%'.format(out_3ds, len(periods), round(100* out_3ds/len(periods),3)))
print('Max Out from mean: {}[s]'.format(max(periods - periods.mean())))
#print('Diff w/expected end: {}[ms]'.format(err[-1]))
#print('Diff by interval: {}[ms]'.format(err[-1]/len(data)))
