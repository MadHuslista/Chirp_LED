# -*- coding: utf-8 -*-
"""
Created on Tue Sep 28 07:32:46 2021

@author: Meyerhof
"""

import numpy as np
import matplotlib.pyplot as plt

args = {
        #General
        "base_amp":1.25,                     # Línea Base
        "t_adap":2,                         # Adaptación entre cada mod
        
        #Square Mod
        "amp_on":2.5,                         # Amp Fase On
        "t_on":3,                           # Tiempo en On
        "t_off":3,                          # Tiempo en Off
        
        #Freq Mod Sinus
        "freq_mod_final_freq":15,           # Freq Final
        "freq_mod_time":15,                 # Tiempo de Mod
        "freq_mod_amp":1.25,                 # Amplitud Media de la Señal de Frecuencia
        "freq_mod_init_phase":2*np.pi,      # Fase de Inicio
        
        #Amp Mod Sinus
        "amp_mod_time":1,                   # Duración del Seno
        "amp_mod_freq":8,                   # Frecuencia base del Seno
        "amp_mod_max":1.25                   # Amplitud Media final. 
        }

sample_rate = 5000      #Hz

#save_path = r"D:\Proyecto_CHIRP_LED\Interface_NIDAQ\logs\signal.txt"
save_path = r"D:\Proyecto_CHIRP_LED\Interface_Prototype\logs\signal.txt"
#save_path = r"D:\Proyecto_CHIRP_LED\Interface_NI6001\logs\signal.txt"
#save_path = r"D:\Proyecto_CHIRP_LED\Calibration_Test\logs\signal.txt"



def chirp_generator(sample_rate, args, splitted=False):
	adap_units = args['base_amp'] * np.ones(int(args['t_adap'] / sample_rate))
	
	on_units = args['amp_on'] * np.ones(int(args['t_on'] / sample_rate))
	off_units = np.zeros(int(args['t_off'] / sample_rate))
	
	freq_acc = args['freq_mod_final_freq'] / args['freq_mod_time']
	freq_time = np.linspace(0, args['freq_mod_time'] - sample_rate, int(args['freq_mod_time'] / sample_rate))
	freq_units = args['freq_mod_amp'] * np.sin(args['freq_mod_init_phase'] + np.pi * freq_acc * np.multiply(freq_time, freq_time))
	freq_units = freq_units + args['base_amp']
	
	amp_time = np.linspace(0, args['amp_mod_time'] - sample_rate, int(args['amp_mod_time'] / sample_rate))
	amp_units = np.sin(2 * np.pi * args['amp_mod_freq'] * amp_time)
	amp_time = np.linspace(0, args['amp_mod_max'], int(args['amp_mod_time'] / sample_rate))
	amp_units = np.multiply(amp_units, amp_time)
	amp_units = amp_units + args['base_amp']

	# TODO: make it adaptable to the chirp parameters
	line_limit = int(np.floor((args['freq_mod_time']**2 - 0.5) * 0.5))
	freq_mod_max_lines = [np.sqrt(0.5 + 2 * i) for i in range(line_limit)]
	freq_mod_min_lines = [0] + [np.sqrt(-0.5 + 2 * i) for i in range(1, line_limit)]
	freq_mod_zero_lines = [np.sqrt(2 * i) for i in range(line_limit)]

	line_limit = int(np.round(args['amp_mod_time'] * args['amp_mod_freq'] - 0.25))
	amp_mod_max_lines = [((0.25 + i) / args['amp_mod_freq']) for i in range(line_limit)]
	amp_mod_min_lines = [0] + [((-0.25 + i + 1) / args['amp_mod_freq']) for i in range(line_limit)] # i + 1 need 

	chirp_signal = {}

	on_off_units = np.concatenate((on_units, off_units))
	chirp_signal['on_off'] = np.array([np.linspace(0, args['t_on'] + args['t_off'] - sample_rate, len(on_off_units)), on_off_units])
	chirp_signal['adap_0'] = np.array([np.linspace(0, args['t_adap'] - sample_rate, len(adap_units)), adap_units])
	chirp_signal['freq'] = np.array([np.linspace(0, args['freq_mod_time'] - sample_rate, len(freq_units)), freq_units])
	chirp_signal['adap_1'] = np.array([np.linspace(0, args['t_adap'] - sample_rate, len(adap_units)), adap_units])
	chirp_signal['amp'] = np.array([np.linspace(0, args['amp_mod_time'] - sample_rate, len(amp_units)), amp_units])
	chirp_signal['adap_2'] = np.array([np.linspace(0, args['t_adap'] - sample_rate, len(adap_units)), adap_units])

	chirp_signal['freq_mod_max_lines'] = np.array(freq_mod_max_lines)
	chirp_signal['freq_mod_min_lines'] = np.array(freq_mod_min_lines)
	chirp_signal['freq_mod_zero_lines'] = np.array(freq_mod_zero_lines)

	chirp_signal['amp_mod_max_lines'] = np.array(amp_mod_max_lines)
	chirp_signal['amp_mod_min_lines'] = np.array(amp_mod_min_lines)

	chirp_signal['full_signal'] = np.concatenate((on_units, off_units, adap_units, freq_units, adap_units, amp_units, adap_units))
	chirp_signal['full_time'] = np.linspace(0, len(chirp_signal['full_signal']) * sample_rate - sample_rate, len(chirp_signal['full_signal']))

	return chirp_signal

t = chirp_generator(1/sample_rate, args, splitted=True)

plt.plot(t['full_time'],t['full_signal'])

np.savetxt(save_path, t['full_signal'], fmt='%.6f')

print(len( t['full_signal']))