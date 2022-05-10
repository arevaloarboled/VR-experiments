library(jsonlite)
s="{"
m_array=fromJSON(results)
for (i in seq(1,nrow(m_array)-1)) {
  s=paste(s,"new double[] {",toString(m_array[i,1]),',',toString(m_array[i,2]),',',toString(m_array[i,3]),'},',sep='');
}
s=paste(s,"new double[] {",toString(m_array[nrow(m_array),1]),',',toString(m_array[nrow(m_array),2]),',',toString(m_array[nrow(m_array),1]),'} }',sep='');
print(s);