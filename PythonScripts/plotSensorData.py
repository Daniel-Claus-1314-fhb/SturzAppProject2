# -*- coding: utf-8 -*-
"""
Created on Sa Oct 08 16:41:59 2016

@author: Daniel

"""
import numpy as np
import matplotlib.pyplot as plt

# Define the filegroup
filename = 'Langzeitmessung2_2016-10-08_12-05-28.csv'

# Uncomment parts of the script which are not in use
filenameParts = filename.split('.')
dataAccelerometer = np.genfromtxt(filenameParts[0] + '_Accelerometer.' + filenameParts[1], delimiter=",", skip_header=1)
dataGyrometer = np.genfromtxt(filenameParts[0] + '_Gyrometer.' + filenameParts[1], delimiter=",", skip_header=1)
dataQuaternion = np.genfromtxt(filenameParts[0] + '_Quaternion.' + filenameParts[1], delimiter=",", skip_header=1)
dataGeolocation = np.genfromtxt(filenameParts[0] + '_Geolocation.' + filenameParts[1], delimiter=",", skip_header=1)

plt.close('all')

#
# Accelerometer
#
fig, (ax1, ax2, ax3) = plt.subplots(3, sharex=False, sharey=True)
fig.suptitle('Accelerometer')

ax1.set_title('x-axis')
ax1.grid(True)
ax1.plot(dataAccelerometer[:,0], dataAccelerometer[:,1], 'r')

ax2.set_title('y-axis')
ax2.grid(True)
ax2.plot(dataAccelerometer[:,0], dataAccelerometer[:,2], 'g')

ax3.set_title('z-axis')
ax3.grid(True)
ax3.plot(dataAccelerometer[:,0], dataAccelerometer[:,3], 'b')

plt.tight_layout()

#
# Gyrometer
#
fig, (ax1, ax2, ax3) = plt.subplots(3, sharex=False, sharey=True)
fig.suptitle('Gyrometer')

ax1.set_title('x-axis')
ax1.grid(True)
ax1.plot(dataGyrometer[:,0], dataGyrometer[:,1], 'r')

ax2.set_title('y-axis')
ax2.grid(True)
ax2.plot(dataGyrometer[:,0], dataGyrometer[:,2], 'g')

ax3.set_title('z-axis')
ax3.grid(True)
ax3.plot(dataGyrometer[:,0], dataGyrometer[:,3], 'b')

plt.tight_layout()

#
# Quaternion
#
fig, (ax1, ax2, ax3, ax4) = plt.subplots(4, sharex=False, sharey=True)
fig.suptitle('Quaternion')

ax1.set_title('angle')
ax1.grid(True)
ax1.plot(dataQuaternion[:,0], dataQuaternion[:,1], 'm')

ax2.set_title('x-axis')
ax2.grid(True)
ax2.plot(dataQuaternion[:,0], dataQuaternion[:,2], 'r')

ax3.set_title('y-axis')
ax3.grid(True)
ax3.plot(dataQuaternion[:,0], dataQuaternion[:,3], 'g')

ax4.set_title('z-axis')
ax4.grid(True)
ax4.plot(dataQuaternion[:,0], dataQuaternion[:,4], 'b')

plt.tight_layout()
plt.show()