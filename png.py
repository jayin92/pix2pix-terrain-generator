#!/usr/bin/env python
# coding: utf-8

# In[20]:


from mpl_toolkits.mplot3d import Axes3D
import matplotlib.pyplot as plt
import numpy as np
import cv2


# In[21]:


im = cv2.imread("Heightmap/Unity_test.png")


# In[22]:


all_height = []
for x in range(257):
    for y in range(257):
        pix = im[x][y]
        height = pix[0] + 256 * pix[1] + 256 ** 2 * pix[2]
        all_height.append(height)


# In[23]:


len(all_height)
all_height = np.array(all_height)


# In[24]:


all_height.resize(257, 257)


# In[ ]:





# In[25]:


p = plt.imshow(all_height)
plt.colorbar(p)
plt.show()

